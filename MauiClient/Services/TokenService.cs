using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiClient.Services
{
    public class TokenService
    {
        public async Task<bool> IsTokenValidAsync()
        {
            var token = await SecureStorage.GetAsync("JwtToken");

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

        public async Task MarkUserAsLoggedInAsync(string token)
        {
            // Store the token securely
            await SecureStorage.SetAsync("JwtToken", token);

            // You could trigger any additional state management here if needed.
        }

        public async Task LogoutAsync()
        {
            // Remove the token from secure storage
            SecureStorage.Remove("JwtToken");
        }

        public async Task<int?> GetLoggedInUserIdAsync()
        {
            var token = await SecureStorage.GetAsync("JwtToken");

            if (string.IsNullOrEmpty(token)) return null;

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
