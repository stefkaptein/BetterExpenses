using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text.Json;
using BetterExpenses.Common.DTO.Auth;
using BetterExpenses.Web.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BetterExpenses.Web
{
    public sealed class ApiAuthenticationStateProvider(ITokenService tokenService) : AuthenticationStateProvider
    {
        private readonly ITokenService _tokenService = tokenService;
        private static readonly AuthenticationState AnonymousUser = GetUserState([]);

        private const string JwtAuthenticationTypeString = "Jwt";
        
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            List<Claim> claims;
            AuthenticationState state;
            if (await _tokenService.AuthTokenValid())
            {
                claims = await _tokenService.GetClaims();
                state = GetAuthenticatedUserState(claims);
            } 
            else if (!await _tokenService.RefreshToken())
            {
                return AnonymousUser;
            }
            else
            {
                claims = await _tokenService.GetClaims();
                state = GetAuthenticatedUserState(claims);
            }
            
            return state;
        }

        /// <summary>
        /// Before calling this method, ensure that the tokens are not null in the LoginResult!
        /// </summary>
        public async Task UpdateLoginState(LoginResult result)
        {
            await _tokenService.SetTokens(result.AuthToken!, result.RefreshToken!);
            var state = await GetAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(state));
        }

        public async Task ResetLoginState()
        {
            await _tokenService.ClearTokens();
            var state = await GetAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(state));
        }

        private static AuthenticationState GetAuthenticatedUserState(IEnumerable<Claim> claims) =>
            GetUserState(claims, JwtAuthenticationTypeString);
        
        private static AuthenticationState GetUserState(IEnumerable<Claim> claims, string? authType = null) =>
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, authType)));
    }
}
