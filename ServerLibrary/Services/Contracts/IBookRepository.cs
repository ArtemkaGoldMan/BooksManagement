using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Services.Contracts
{
    public interface IBookRepository
    {
        Task<PagedResponse<BookDTO>> GetBooksAsync(string? search, string? sortBy, bool ascending, int pageNumber, int pageSize);
        Task<BookDTO?> GetBookByIdAsync(int id);
        Task<BookDTO> AddBookAsync(CreateBookDTO book);
        Task<BookDTO?> UpdateBookAsync(int id, UpdateBookDTO updatedBook);
        Task<bool> DeleteBookAsync(int id);
        Task<int> GetTotalCountAsync();

        // Borrowing-related methods
        Task<BorrowDTO?> BorrowBookAsync(BorrowRequestDTO request);
        Task<bool> ReturnBookAsync(int bookId, int userId);
        Task<List<BorrowDTO>> GetBorrowedBooksAsync(int userId);
        Task<List<BorrowDTO>> GetAllBorrowedBooksAsync(); // For admin view

    }
}
