using BaseLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiClient.Services
{
    public class BorrowHistoryService
    {
        private const string BaseUrl = "https://localhost:7152/api";

        private readonly HttpClient _httpClient;

        public BorrowHistoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<BorrowHistoryDTO>> GetBorrowHistoryAsync()
        {
            try
            {
                var token = await SecureStorage.Default.GetAsync("JwtToken");
                if (string.IsNullOrEmpty(token))
                {
                    throw new Exception("No JWT token found.");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"{BaseUrl}/books/history");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<BorrowHistoryDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching borrow history: {ex.Message}");
                return new List<BorrowHistoryDTO>();
            }
        }


        public async Task<List<BorrowHistoryDTO>> GetNotReturnedBooksAsync()
        {
            try
            {
                var allHistory = await GetBorrowHistoryAsync();
                return allHistory?.FindAll(b => b.ReturnDate == null) ?? new List<BorrowHistoryDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching borrow history: {ex.Message}");
                return new List<BorrowHistoryDTO>();
            }
        }
    }
}
