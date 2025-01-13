using BaseLibrary.DTOs;
using BaseLibrary.Response;
using BaseLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WPFClient.Helpers;
using System.Net.Http.Json;
using System.Windows;
using System.Net;

namespace WPFClient.Services
{
    public class BooksService : IBooksService
    {
        private readonly HttpClient _httpClient;
        public BooksService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7152/api/") };
        }


        // KAK U ARTEMA
        // Get all books from the API
        public async Task<PagedResponse<BookDTO>> GetBooksAsync(string? search = null, string? sortBy = null, bool ascending = true, int pageNumber = 1, int pageSize = 10)
        {
            await SetAuthorizationHeader();
            var response = await _httpClient.GetFromJsonAsync<PagedResponse<BookDTO>>(
                $"Books?search={search}&sortBy={sortBy}&ascending={ascending}&pageNumber={pageNumber}&pageSize={pageSize}");
            return response ?? new PagedResponse<BookDTO>();
        }




        // KAK U ARTEMA
        public async Task<BookDTO?> GetBookAsync(int id)
        {
            await SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<BookDTO>($"Books/{id}");
        }




        // KAK U ARTEMA
        // Get borrowed books for the user
        public async Task<List<BorrowDTO>?> GetBorrowedBooksByUserAsync(int userId)
        {
            await SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<BorrowDTO>>($"Books/borrowed/{userId}");
        }


        // KAK U ARTEMA
        // Borrow a book using BorrowRequestDTO   
        public async Task<bool> BorrowBookAsync(BorrowRequestDTO request)
        {
            await SetAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("Books/borrow", request);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"BorrowBook failed: {error}");
            return false;
        }



        // KAK U ARTEMA
        public async Task<List<BorrowDTO>> GetAllBorrowedBooksAsync()
        {
            try
            {
                await SetAuthorizationHeader();
                var response = await _httpClient.GetAsync("Books/borrowed");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<BorrowDTO>>() ?? new List<BorrowDTO>();
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("Unauthorized access to GetAllBorrowedBooks");
                }
                else
                {
                    Console.WriteLine($"Error fetching borrowed books: {response.ReasonPhrase}");
                }

                return new List<BorrowDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching borrowed books: {ex.Message}");
                return new List<BorrowDTO>();
            }
        }


        //KAK U ARTEMA
        // Return a book using query parameters
        public async Task<bool> ReturnBookAsync(int bookId, int userId)
        {
            await SetAuthorizationHeader();
            var response = await _httpClient.PostAsync($"Books/return?bookId={bookId}&userId={userId}", null);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"ReturnBook failed: {error}");
            return false;
        }


        //KAK U ARTEMA
        public async Task<List<BorrowHistoryDTO>> GetBorrowHistoryAsync()
        {
            try
            {
                await SetAuthorizationHeader();
                var response = await _httpClient.GetAsync("Books/history");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<BorrowHistoryDTO>>() ?? new List<BorrowHistoryDTO>();
                }
                else
                {
                    Console.WriteLine($"Error fetching borrow history: {response.ReasonPhrase}");
                    return new List<BorrowHistoryDTO>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching borrow history: {ex.Message}");
                return new List<BorrowHistoryDTO>();
            }
        }



        //KAK U ARTEMA
        // (Admin only)
        public async Task<bool> AddBookAsync(CreateBookDTO createBookDto)
        {
            await SetAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("Books", createBookDto);
            return response.IsSuccessStatusCode;
        }


        //KAK U ARTEMA
        //(Admin only)
        public async Task<bool> UpdateBookAsync(int id, UpdateBookDTO updateBookDto)
        {
            await SetAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"Books/{id}", updateBookDto);
            return response.IsSuccessStatusCode;
        }


        //KAK U ARTEMA
        //  (Admin only)
        public async Task<bool> DeleteBookAsync(int id)
        {
            await SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"Books/{id}");
            return response.IsSuccessStatusCode;
        }



        // Authorization header with the token
        private async Task SetAuthorizationHeader()
        {
            if (TokenStorage.IsAuthenticated)
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenStorage.Token);
            }
        }
    }
}