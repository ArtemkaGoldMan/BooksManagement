using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Response;
using Blazored.LocalStorage;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace WebClientBlazor.Services
{
    public class BookService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public BookService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        // Get all books with search, sorting, and pagination
        public async Task<PagedResponse<BookDTO>> GetBooksAsync(string? search, string? sortBy, bool ascending, int pageNumber, int pageSize)
        {
            await SetAuthorizationHeader();
            var response = await _httpClient.GetFromJsonAsync<PagedResponse<BookDTO>>(
                $"api/Books?search={search}&sortBy={sortBy}&ascending={ascending}&pageNumber={pageNumber}&pageSize={pageSize}");
            return response ?? new PagedResponse<BookDTO>();
        }

        // Get a book by ID
        public async Task<BookDTO?> GetBookAsync(int id)
        {
            await SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<BookDTO>($"api/Books/{id}");
        }

        // Create a new book (Admin only)
        public async Task<bool> CreateBookAsync(CreateBookDTO createBookDto)
        {
            await SetAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/Books", createBookDto);
            return response.IsSuccessStatusCode;
        }

        // Update an existing book (Admin only)
        public async Task<bool> UpdateBookAsync(int id, UpdateBookDTO updateBookDto)
        {
            await SetAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"api/Books/{id}", updateBookDto);
            return response.IsSuccessStatusCode;
        }

        // Delete a book (Admin only)
        public async Task<bool> DeleteBookAsync(int id)
        {
            await SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/Books/{id}");
            return response.IsSuccessStatusCode;
        }

        // Borrow a book
        public async Task<bool> BorrowBookAsync(BorrowRequestDTO request)
        {
            await SetAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/Books/borrow", request);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            // Log or handle the error message from the server
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"BorrowBook failed: {error}");
            return false;
        }


        // Return a borrowed book
        public async Task<bool> ReturnBookAsync(int bookId, int userId)
        {
            await SetAuthorizationHeader();
            var response = await _httpClient.PostAsync($"api/Books/return?bookId={bookId}&userId={userId}", null);
            return response.IsSuccessStatusCode;
        }

        // Get all books borrowed by a specific user
        public async Task<List<BorrowDTO>?> GetBorrowedBooksByUserAsync(int userId)
        {
            await SetAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<BorrowDTO>>($"api/Books/borrowed/{userId}");
        }

        private async Task SetAuthorizationHeader()
        {
            var token = await _localStorage.GetItemAsync<string>("token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }


        public async Task<List<BorrowDTO>> GetAllBorrowedBooksAsync()
        {
            try
            {
                await SetAuthorizationHeader();
                var response = await _httpClient.GetAsync("api/Books/borrowed");

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
        public async Task<List<BorrowHistoryDTO>> GetBorrowHistoryAsync()
        {
            try
            {
                await SetAuthorizationHeader();
                var response = await _httpClient.GetAsync("api/Books/history");

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


    }
}
