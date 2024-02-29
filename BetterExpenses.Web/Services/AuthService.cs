using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using BetterExpenses.Common.DTO.Auth;
using BetterExpenses.Common.Extensions;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BetterExpenses.Web.Services;

public interface IAuthService
{
    Task<LoginResult> Login(LoginModel loginModel);
    Task Logout();
    Task<RegisterResult> Register(RegisterModel registerModel);
}

public class AuthService(
    HttpClient httpClient,
    AuthenticationStateProvider authenticationStateProvider,
    ILocalStorageService localStorage,
    ITokenService tokenService)
    : IAuthService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider = authenticationStateProvider;
    private readonly ILocalStorageService _localStorage = localStorage;
    private readonly ITokenService _tokenService = tokenService;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<RegisterResult> Register(RegisterModel registerModel)
    {
        var result = await _httpClient.PostAsJsonAsync("/api/auth/register", registerModel);
        var jsonString = await result.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<RegisterResult>(jsonString, JsonSerializerOptions) ??
               throw new Exception("Cannot deserialize result");
    }

    public async Task<LoginResult> Login(LoginModel loginModel)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginModel);
        var loginResult =
            JsonSerializer.Deserialize<LoginResult>(await response.Content.ReadAsStringAsync(), JsonSerializerOptions);

        if (loginResult == null)
        {
            throw new Exception("Cannot deserialize result");
        }
        
        if (!response.IsSuccessStatusCode || loginResult.AuthToken == null || loginResult.RefreshToken == null)
        {
            return loginResult;
        }

        await _tokenService.SetTokens(loginResult.AuthToken, loginResult.RefreshToken);

        return loginResult;
    }

    public async Task Logout()
    {
        await _tokenService.ClearTokens();
    }
}