using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using BetterExpenses.Common.DTO.Auth;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BetterExpenses.Web
{
    public class ApiAuthenticationStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
        : AuthenticationStateProvider
    {
        private static readonly ClaimsPrincipal AnonymousUser = new(new ClaimsIdentity());

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = await localStorage.GetItemAsync<TokenWithExperation>("authToken");

            if (savedToken == null || string.IsNullOrWhiteSpace(savedToken.Token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken.Token);

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(savedToken.Token), "jwt")));
        }

        public void SetJwtToken(IEnumerable<Claim> claims)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "apiauth"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void LogoutUser()
        {
            var authState = Task.FromResult(new AuthenticationState(AnonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
