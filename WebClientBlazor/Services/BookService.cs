using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Response;
using System.Net.Http.Json;

namespace WebClientBlazor.Services
{
    public class BookService
    {
        private readonly HttpClient _httpClient;

        public BookService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Get all books with search, sorting, and pagination
        public async Task<PagedResponse<BookDTO>> GetBooksAsync(string? search, string? sortBy, bool ascending, int pageNumber, int pageSize)
        {
            var response = await _httpClient.GetFromJsonAsync<PagedResponse<BookDTO>>(
                $"api/Books?search={search}&sortBy={sortBy}&ascending={ascending}&pageNumber={pageNumber}&pageSize={pageSize}");
            return response ?? new PagedResponse<BookDTO>();
        }

        // Get a book by ID
        public async Task<BookDTO?> GetBookAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<BookDTO>($"api/Books/{id}");
        }

        // Create a new book (Admin only)
        public async Task<bool> CreateBookAsync(CreateBookDTO createBookDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Books", createBookDto);
            return response.IsSuccessStatusCode;
        }

        // Update an existing book (Admin only)
        public async Task<bool> UpdateBookAsync(int id, UpdateBookDTO updateBookDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Books/{id}", updateBookDto);
            return response.IsSuccessStatusCode;
        }

        // Delete a book (Admin only)
        public async Task<bool> DeleteBookAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Books/{id}");
            return response.IsSuccessStatusCode;
        }

        // Borrow a book
        public async Task<bool> BorrowBookAsync(BorrowRequestDTO request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Books/borrow", request);
            return response.IsSuccessStatusCode;
        }

        // Return a borrowed book
        public async Task<bool> ReturnBookAsync(int bookId, int userId)
        {
            var response = await _httpClient.PostAsync($"api/Books/return?bookId={bookId}&userId={userId}", null);
            return response.IsSuccessStatusCode;
        }

        // Get all books borrowed by a specific user
        public async Task<List<BorrowDTO>?> GetBorrowedBooksByUserAsync(int userId)
        {
            return await _httpClient.GetFromJsonAsync<List<BorrowDTO>>($"api/Books/borrowed/{userId}");
        }

        // Get all borrowed books (Admin only)
        public async Task<List<BorrowDTO>?> GetAllBorrowedBooksAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<BorrowDTO>>("api/Books/borrowed");
        }
    }
}
