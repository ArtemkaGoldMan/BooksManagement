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
        Task<PagedResponse<Book>> GetBooksAsync(string? search, string? sortBy, bool ascending, int pageNumber, int pageSize);
        Task<Book?> GetBookByIdAsync(int id);
        Task<Book> AddBookAsync(Book book);
        Task<Book?> UpdateBookAsync(int id, Book updatedBook);
        Task<bool> DeleteBookAsync(int id);
        Task<int> GetTotalCountAsync();
    }
}
