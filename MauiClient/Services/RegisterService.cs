using BaseLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiClient.Services
{
    public class RegistrationService
    {
        private const string BaseUrl = "https://localhost:7152/api";

        public async Task<bool> RegisterAsync(RegisterUserDTO registerDto)
        {
            using var client = new HttpClient();
            var json = JsonSerializer.Serialize(registerDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{BaseUrl}/users/register", content);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Registration failed: {errorResponse}");
            return false;
        }
    }
}
