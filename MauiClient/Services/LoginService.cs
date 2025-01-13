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
            try
            {
                using var client = new HttpClient();
                var json = JsonSerializer.Serialize(loginDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{BaseUrl}/users/login", content);

                if (!response.IsSuccessStatusCode)
                {
                    // Log the error response
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {errorResponse}");
                    return null;
                }

                var responseData = await response.Content.ReadAsStringAsync();
                var token = JsonSerializer.Deserialize<JsonElement>(responseData)
                    .GetProperty("token")
                    .GetString();

                if (!string.IsNullOrEmpty(token))
                {
                    await _tokenService.MarkUserAsLoggedInAsync(token);
                }

                return token;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Exception in LoginAsync: {ex.Message}");
                return null;
            }
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
