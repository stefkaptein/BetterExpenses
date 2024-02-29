using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using BetterExpenses.Common.DTO.Auth;
using BetterExpenses.Common.Extensions;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace BetterExpenses.Web.Services;

internal record Tokens(string AuthToken, DateTime AuthTokenExpires, string RefreshToken, DateTime RefreshTokenExpires);

public interface ITokenService
{
    public Task SetTokens(string authToken, TokenWithExperation refreshToken);
    public Task<bool> AuthTokenValid();
    public Task<bool> RefreshToken(Guid userId);
    public Task ClearTokens();
}

public class TokenService(
    HttpClient httpClient,
    AuthenticationStateProvider authenticationStateProvider,
    ILocalStorageService localStorage) : ITokenService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider = authenticationStateProvider;
    private readonly ILocalStorageService _localStorage = localStorage;
    
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    private const string TokensInternalStorageName = "tokens";
    
    public async Task SetTokens(string authToken, TokenWithExperation refreshToken)
    {
        var claims = ParseClaimsFromJwt(authToken);
        var expires = claims.GetTokenExpiration();

        var tokens = new Tokens(authToken, expires, refreshToken.Token, refreshToken.Expires);
        
        await _localStorage.SetItemAsync(TokensInternalStorageName, tokens);
        
        ((ApiAuthenticationStateProvider)_authenticationStateProvider).SetJwtToken(authToken);
        
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("bearer", authToken);
    }

    public async Task<bool> AuthTokenValid()
    {
        var tokens = await _localStorage.GetItemAsync<Tokens>(TokensInternalStorageName);
        return tokens?.AuthTokenExpires < DateTime.UtcNow;
    }

    private static bool RefreshTokenValid(Tokens? tokens) => tokens?.RefreshTokenExpires < DateTime.UtcNow;


    public async Task<bool> RefreshToken(Guid userId)
    {
        var tokens = await _localStorage.GetItemAsync<Tokens>(TokensInternalStorageName);
        
        if (!RefreshTokenValid(tokens))
        {
            return false;
        }

        var refreshTokenModel = new RefreshTokenModel
        {
            RefreshToken = tokens!.RefreshToken,
            UserId = userId
        };
        
        var response = await _httpClient.PostAsJsonAsync("/api/auth/refresh", refreshTokenModel);
        var loginResult =
            JsonSerializer.Deserialize<LoginResult>(await response.Content.ReadAsStringAsync(), JsonSerializerOptions);

        if (loginResult?.Successful != true)
        {
            return false;
        }

        await SetTokens(loginResult.AuthToken!, loginResult.RefreshToken!);
        return true;
    }

    public async Task ClearTokens()
    {
        await _localStorage.RemoveItemAsync(TokensInternalStorageName);
        ((ApiAuthenticationStateProvider)_authenticationStateProvider).LogoutUser();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    private static List<Claim> ParseClaimsFromJwt(string jwt)
    {
        var handler = new JsonWebTokenHandler();
        var token = handler.ReadJsonWebToken(jwt);
        return token.Claims.ToList();
    }
}