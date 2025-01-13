using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BaseLibrary.DTOs;

namespace MauiClient.Services
{
    public class LoginService
    {
        private const string BaseUrl = "https://localhost:7152/api";
        private readonly TokenService _tokenService;

        public LoginService()
        {
            _tokenService = new TokenService();
        }

        public async Task<string> LoginAsync(LoginUserDTO loginDto)
        {
            using var client = new HttpClient();
            var json = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{BaseUrl}/users/login", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var responseData = await response.Content.ReadAsStringAsync();
            var jsonData = JsonSerializer.Deserialize<JsonElement>(responseData);

            var token = jsonData.GetProperty("token").GetString();
            if (!string.IsNullOrEmpty(token))
            {
               
                var claims = ParseClaimsFromJwt(token);


                var role = claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

                await SecureStorage.SetAsync("JwtToken", token);

                if (!string.IsNullOrEmpty(role))
                {
                    await SecureStorage.SetAsync("UserRole", role);
                }
            }

            return token;
        }

        private IEnumerable<System.Security.Claims.Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            return keyValuePairs.Select(kvp => new System.Security.Claims.Claim(kvp.Key, kvp.Value.ToString()));
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            base64 = base64.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }


        public async Task<bool> IsAuthenticatedAsync()
        {
            return await _tokenService.IsTokenValidAsync();
        }

        public async Task LogoutAsync()
        {
            await _tokenService.LogoutAsync();
        }
    }
}
