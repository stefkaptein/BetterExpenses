using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using BetterExpenses.Common.DTO.Auth;
using BetterExpenses.Common.Extensions;
using Blazored.LocalStorage;
using Bunq.Sdk.Model.Generated.Endpoint;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using MongoDB.Bson.Serialization.Serializers;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace BetterExpenses.Web.Services;

internal record Tokens(string AuthToken, DateTime AuthTokenExpires, string RefreshToken, DateTime RefreshTokenExpires);

public interface ITokenService
{
    public Task SetTokens(string authToken, TokenWithExperation refreshToken);
    public Task<List<Claim>> GetClaims();
    public Task<bool> AuthTokenValid();
    public Task<bool> RefreshToken();
    public Task ClearTokens();
}

public class TokenService(HttpClient httpClient, ILocalStorageService localStorage) : ITokenService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILocalStorageService _localStorage = localStorage;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    private const string TokensInternalStorageName = "tokens";
    private const string UserIdInternalStorageName = "userId";

    public async Task SetTokens(string authToken, TokenWithExperation refreshToken)
    {
        var claims = ParseClaimsFromJwt(authToken);
        var expires = claims.GetTokenExpiration();
        var userId = claims.GetUserId();

        var tokens = new Tokens(authToken, expires, refreshToken.Token, refreshToken.Expires);

        await _localStorage.SetItemAsync(TokensInternalStorageName, tokens);
        await _localStorage.SetItemAsStringAsync(UserIdInternalStorageName, userId!.ToString()!);

        SetAuthenticationHeader(authToken);
    }

    public async Task<List<Claim>> GetClaims()
    {
        var tokensFromStorage = await _localStorage.GetItemAsync<Tokens>(TokensInternalStorageName);
        return tokensFromStorage == null ? [] : ParseClaimsFromJwt(tokensFromStorage.AuthToken);
    }

    public async Task<bool> AuthTokenValid()
    {
        var tokens = await _localStorage.GetItemAsync<Tokens>(TokensInternalStorageName);
        if (tokens?.AuthTokenExpires < DateTime.UtcNow)
        {
            SetAuthenticationHeader(tokens.AuthToken);
        }

        return false;
    }

    private static bool RefreshTokenValid(Tokens? tokens) => tokens?.RefreshTokenExpires > DateTime.UtcNow;

    public async Task<bool> RefreshToken()
    {
        var tokens = await _localStorage.GetItemAsync<Tokens>(TokensInternalStorageName);

        if (!RefreshTokenValid(tokens))
        {
            return false;
        }

        var userIdStr = await _localStorage.GetItemAsStringAsync(UserIdInternalStorageName);

        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
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
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    private void SetAuthenticationHeader(string authToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("bearer", authToken);
    }

    private static List<Claim> ParseClaimsFromJwt(string jwt)
    {
        var handler = new JsonWebTokenHandler();
        var token = handler.ReadJsonWebToken(jwt);
        return token.Claims.ToList();
    }
}