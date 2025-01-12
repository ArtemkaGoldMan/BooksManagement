using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using WPFClient.Helpers;
using BaseLibrary.DTOs;

namespace WPFClient.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;

        // Constructor with base URL setup for the API
        public AuthenticationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7152/api/");
        }


        public ClaimsPrincipal GetAuthenticationState()
        {
            // Get the token from your TokenStorage
            var token = TokenStorage.Token;

            if (string.IsNullOrEmpty(token))
            {
                // Return an unauthenticated state (empty ClaimsPrincipal)
                return new ClaimsPrincipal(new ClaimsIdentity());
            }

            // Parse the claims from the JWT token
            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return user;
        }

        public async Task MarkUserAsLoggedIn(string token)
        {
            // Store the token in the TokenStorage (you can also store it in local storage if needed)
            TokenStorage.Token = token;

            // Parse the claims from the JWT token
            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            // Set Authorization header for future requests (optional if you plan to send token in requests)
            SetAuthorizationHeader(user);
        }


        // Login method, which sends the login request and stores the JWT token
        public async Task<bool> LoginAsync(LoginUserDTO loginDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Users/login", loginDto);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the token response
                    var result = await response.Content.ReadFromJsonAsync<TokenResponse>();

                    // Store the token
                    TokenStorage.Token = result?.Token;

                    // Mark the user as logged in (this will parse the claims and set the user)
                    if (result?.Token != null)
                    {
                        await MarkUserAsLoggedIn(result.Token);  // Use the MarkUserAsLoggedIn method to set up claims.

                    }

                    // Set the Authorization header for future requests
                    SetAuthorizationHeader(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(result.Token), "jwt")));

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login: {ex.Message}");
                return false;
            }
        }

        // Register method to send a registration request
        public async Task<bool> RegisterAsync(RegisterUserDTO registerDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Users/register", registerDto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Handle any registration-specific errors
                Console.WriteLine($"Error during registration: {ex.Message}");
                return false;
            }
        }



        // Ensure that all outgoing requests have the Authorization header if the user is logged in
        public void SetAuthorizationHeader(ClaimsPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {

                var token = TokenStorage.Token;
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
            }
        }


        // Optionally, you could implement a logout method to clear the token
        public void Logout()
        {
            TokenStorage.ClearToken();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        // Helper method to check if the user is authenticated (i.e., token exists)
        public bool IsAuthenticated() => TokenStorage.IsAuthenticated;


        public async Task<bool> DeleteUserAsync(int userId)
        {
            var response = await _httpClient.DeleteAsync($"api/Users/{userId}");
            return response.IsSuccessStatusCode;
        }

        public IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            try
            {
                var payload = jwt.Split('.')[1];
                var jsonBytes = Convert.FromBase64String(Base64UrlDecode(payload));
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

                return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
            }
            catch (Exception ex)
            {
                // Log the error or handle as needed
                Console.WriteLine($"Error parsing JWT: {ex.Message}");
                return Enumerable.Empty<Claim>();
            }
        }


        public string Base64UrlDecode(string input)
        {
            input = input.Replace('-', '+').Replace('_', '/');
            switch (input.Length % 4)
            {
                case 2: input += "=="; break;
                case 3: input += "="; break;
            }
            return input;
        }


        public async Task<bool> IsTokenValidAsync()
        {
            var token = TokenStorage.Token;

            if (string.IsNullOrEmpty(token)) return false;

            var claims = ParseClaimsFromJwt(token);
            var expiryClaim = claims.FirstOrDefault(c => c.Type == "exp")?.Value;

            if (expiryClaim != null)
            {
                var expiryDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiryClaim)).UtcDateTime;
                return expiryDate > DateTime.UtcNow;
            }

            return false;
        }

    }

    // DTO class to represent the response from the login API with the token
    public class TokenResponse
    {
        public string Token { get; set; }
    }
}
