using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace WebClientBlazor.Helpers
{
    public class TokenAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public TokenAuthenticationStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Get the token from local storage
            var token = await _localStorage.GetItemAsync<string>("token");

            if (string.IsNullOrEmpty(token))
            {
                // Return an unauthenticated state
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // Parse the token and create the authenticated user
            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }

        public async Task MarkUserAsLoggedIn(string token)
        {
            // Store the token in local storage
            await _localStorage.SetItemAsync("token", token);

            // Parse the claims from the token
            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            // Notify the authentication state change
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task Logout()
        {
            // Remove the token from local storage
            await _localStorage.RemoveItemAsync("token");

            // Notify Blazor that the user is logged out
            var anonymousUser = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            NotifyAuthenticationStateChanged(Task.FromResult(anonymousUser));
        }

        public async Task<int?> GetLoggedInUserIdAsync()
        {
            // Get the token from local storage
            var token = await _localStorage.GetItemAsync<string>("token");

            if (string.IsNullOrEmpty(token))
            {
                return null; // User is not logged in
            }

            var claims = ParseClaimsFromJwt(token);
            var userIdClaim = claims.FirstOrDefault(c => c.Type == "UserId");

            return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = Convert.FromBase64String(Base64UrlDecode(payload));
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }

        private string Base64UrlDecode(string input)
        {
            input = input.Replace('-', '+').Replace('_', '/');
            switch (input.Length % 4)
            {
                case 2: input += "=="; break;
                case 3: input += "="; break;
            }
            return input;
        }
    }
}
