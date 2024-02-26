using BetterExpenses.API.Services.Options;
using BetterExpenses.Common.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace BetterExpenses.API.Services.Auth;

public interface IBunqAuthService
{
    public string GetAuthUri(Guid userId);
    public bool TryGetUserIdForState(Guid state, out Guid userId);
    public Task<string> GetAccessToken(string authCode);
}

public class BunqAuthService(
    IOptions<AuthOptions> authOptions,
    IOptions<OAuthClientOptions> oauthClientOptions,
    IOptions<BunqOptions> bunqOptions) : IBunqAuthService
{
    private readonly string _clientId = oauthClientOptions.Value.ClientId;
    private readonly string _clientSecret = oauthClientOptions.Value.ClientSecret;
    private readonly string _redirectUri = authOptions.Value.RedirectUri;

    private readonly Uri _baseOauthApiUri = new(authOptions.Value.BaseUri);
    private readonly Uri _baseBunqApiUri = new(bunqOptions.Value.BaseApiPath);
    private readonly TimeSpan _stateExpires = TimeSpan.FromMinutes(int.Parse(bunqOptions.Value.StateExpiresMinutes));

    private Uri AuthUri => new(_baseOauthApiUri, authOptions.Value.AuthRoute);
    private Uri TokenUri => new(_baseBunqApiUri, authOptions.Value.TokenRoute);

    private readonly MemoryCache _stateUserIdCache = new(new MemoryCacheOptions( ));
    private readonly HttpClient _httpClient = new();

    public string GetAuthUri(Guid userId)
    {
        var uriBuilder = new UriBuilder(AuthUri);
            
        var stateId = Guid.NewGuid();
        using (var entry = _stateUserIdCache.CreateEntry(stateId))
        {
            entry.Value = userId;
            entry.SlidingExpiration = _stateExpires;
        }
        
        // Add query parameters
        var queryParams = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
        queryParams["response_type"] = "code";
        queryParams["client_id"] = _clientId;
        queryParams["redirect_uri"] = _redirectUri;
        queryParams["state"] = stateId.ToString();

        uriBuilder.Query = queryParams.ToString();

        return uriBuilder.ToString();
    }

    public bool TryGetUserIdForState(Guid state, out Guid userId) => _stateUserIdCache.TryGetValue(state, out userId);

    public async Task<string> GetAccessToken(string authCode)
    {
        var uriBuilder = new UriBuilder(TokenUri);

        // Add query parameters
        var queryParams = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
        queryParams["grant_type"] = "authorization_code";
        queryParams["code"] = authCode;
        queryParams["client_id"] = _clientId;
        queryParams["client_secret"] = _clientSecret;
        queryParams["redirect_uri"] = _redirectUri;

        uriBuilder.Query = queryParams.ToString();
        var url = uriBuilder.ToString();

        var response = await _httpClient.PostAsync(url, null);
        var responseValues = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

        return responseValues["access_token"];
    }
}