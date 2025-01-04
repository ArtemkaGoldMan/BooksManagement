using System.Net.Http.Json;
using BaseLibrary.DTOs;

namespace WebClientBlazor.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> LoginAsync(LoginUserDTO loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Users/login", loginDto);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                return result?["token"];
            }
            return null;
        }

        public async Task<bool> RegisterAsync(RegisterUserDTO registerDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Users/register", registerDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<UserDTO>> GetUsersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<UserDTO>>("api/Users");
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var response = await _httpClient.DeleteAsync($"api/Users/{userId}");
            return response.IsSuccessStatusCode;
        }
    }
}
