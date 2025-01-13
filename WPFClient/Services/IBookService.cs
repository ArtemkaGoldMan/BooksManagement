using BaseLibrary.DTOs;
using BaseLibrary.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient.Services
{
    public interface IBooksService
    {
        //Task<List<BookDTO>> GetBooksAsync();

        // Get all books with optional search, sorting, pagination
        Task<PagedResponse<BookDTO>> GetBooksAsync(string? search = null, string? sortBy = null, bool ascending = true, int pageNumber = 1, int pageSize = 10);

        // Get a specific book by its ID
        Task<BookDTO?> GetBookAsync(int id);

        // Get borrowed books for a specific user
        // Task<List<BorrowDTO>?> GetBorrowedBooksByUserAsync(int userId);

        Task<List<BorrowDTO>?> GetBorrowedBooksByUserAsync(int userId);

        // Borrow a book by sending a BorrowRequestDTO
        Task<bool> BorrowBookAsync(BorrowRequestDTO request);

        // Get all borrowed books
        Task<List<BorrowDTO>> GetAllBorrowedBooksAsync();

        // Return a borrowed book using its ID and user ID
        Task<bool> ReturnBookAsync(int bookId, int userId);

        Task<bool> DeleteBookAsync(int id);


        Task<bool> UpdateBookAsync(int id, UpdateBookDTO updateBookDto);


        Task<bool> AddBookAsync(CreateBookDTO createBookDto);


        Task<List<BorrowHistoryDTO>> GetBorrowHistoryAsync();
    }
}
