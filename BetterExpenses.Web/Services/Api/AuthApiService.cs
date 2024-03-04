using System.Net;
using System.Net.Http.Json;
using BetterExpenses.Common.DTO.Auth;
using BetterExpenses.Web.Json;
using BetterExpenses.Web.Models.Options;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;

namespace BetterExpenses.Web.Services.Api;

public interface IAuthApiService
{
    public Task<LoginResult> Login(LoginModel loginModel);
    public Task<RegisterResult> Register(RegisterModel registerModel);
    public Task<LoginResult?> RefreshToken(RefreshTokenModel refreshTokenModel);
    public Task<string?> GetLinkBunqAuthUrl();
    public Task<bool> UnlinkBunq();
    public Task<bool> GetAuthToken(string code, string state);
}

public class AuthApiApiService(
    IOptions<AuthOptions> authOptions,
    HttpClient httpClient)
    : ApiService(httpClient), IAuthApiService
{
    private const string RegisterPath = "/api/auth/register";
    private const string LoginPath = "/api/auth/login";
    private const string RefreshTokenPath = "/api/auth/refresh";
    private const string LinkBunqPath = "/api/auth/linkBunq";
    private const string UnLinkBunqPath = "/api/auth/unLinkBunq";
    private const string LinkBunqCallbackPath = "/api/auth/callback";

    private readonly Dictionary<string, string> _callbackUrlParams = new()
    {
        { "callback", authOptions.Value.CallbackUrl }
    };

    public async Task<RegisterResult> Register(RegisterModel registerModel)
    {
        return (await PostModel<RegisterResult, RegisterModel>(RegisterPath, registerModel)).ResponseModel;
    }

    public async Task<LoginResult> Login(LoginModel loginModel)
    {
        var apiResponse = await PostModel<LoginResult, LoginModel>(LoginPath, loginModel);
        var loginResult = apiResponse.ResponseModel;

        if (!apiResponse.IsSuccessStatusCode || loginResult.AuthToken == null || loginResult.RefreshToken == null)
        {
            return loginResult;
        }

        return loginResult;
    }

    public async Task<LoginResult?> RefreshToken(RefreshTokenModel refreshTokenModel)
    {
        var response = await PostModel<LoginResult, RefreshTokenModel>(RefreshTokenPath, refreshTokenModel);
        return response.IsSuccessStatusCode ? response.ResponseModel : null;
    }

    public async Task<string?> GetLinkBunqAuthUrl()
    {
        var response = await Get(LinkBunqPath, _callbackUrlParams);
        if (response.IsSuccessStatusCode && response.Content != null)
        {
            return response.Content;
        }

        return null;
    }

    public async Task<bool> UnlinkBunq()
    {
        var response = await Get(LinkBunqPath, _callbackUrlParams);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> GetAuthToken(string code, string state)
    {
        var response = await Get(LinkBunqCallbackPath, GetCallbackDictionary(code, state));
        return response.IsSuccessStatusCode;
    }

    private Dictionary<string, string> GetCallbackDictionary(string code, string state) => new()
    {
        { "code", code },
        { "state", state },
        { "callback", authOptions.Value.CallbackUrl }
    };
}