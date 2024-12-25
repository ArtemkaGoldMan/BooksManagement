using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Response;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Services.Implementations
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;

        public BookRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get Books with DTO
        public async Task<PagedResponse<BookDTO>> GetBooksAsync(string? search, string? sortBy, bool ascending, int pageNumber, int pageSize)
        {
            var query = _context.Books.AsQueryable();

            // Total count before applying filters
            int totalCount = await query.CountAsync();

            // Filtering
            if (!string.IsNullOrEmpty(search))
                query = query.Where(b => b.Title.Contains(search) || b.Author.Contains(search) || b.Genre.Contains(search));

            // Filtered count after applying filters
            int filteredCount = await query.CountAsync();

            // Sorting
            query = sortBy switch
            {
                "Title" => ascending ? query.OrderBy(b => b.Title) : query.OrderByDescending(b => b.Title),
                "Price" => ascending ? query.OrderBy(b => b.Price) : query.OrderByDescending(b => b.Price),
                "Genre" => ascending ? query.OrderBy(b => b.Genre) : query.OrderByDescending(b => b.Genre),
                "Author" => ascending ? query.OrderBy(b => b.Author) : query.OrderByDescending(b => b.Author),
                _ => query.OrderBy(b => b.Id) // Default case: no sorting
            };

            // Pagination
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
                .Select(b => new BookDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Price = b.Price,
                    PublishedDate = b.PublishedDate,
                    Genre = b.Genre
                })
                .ToListAsync();

            // Return both items and counts
            return new PagedResponse<BookDTO>
            {
                Items = items,
                TotalCount = totalCount,
                FilteredCount = filteredCount
            };
        }


        public async Task<BookDTO?> GetBookByIdAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return null;

            return new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Price = book.Price,
                PublishedDate = book.PublishedDate,
                Genre = book.Genre
            };
        }

        public async Task<BookDTO> AddBookAsync(CreateBookDTO bookDto)
        {
            var book = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                Price = bookDto.Price,
                PublishedDate = bookDto.PublishedDate,
                Genre = bookDto.Genre
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Price = book.Price,
                PublishedDate = book.PublishedDate,
                Genre = book.Genre
            };
        }

        public async Task<BookDTO?> UpdateBookAsync(int id, UpdateBookDTO updatedBookDto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return null;

            book.Title = updatedBookDto.Title;
            book.Author = updatedBookDto.Author;
            book.Price = updatedBookDto.Price;
            book.PublishedDate = updatedBookDto.PublishedDate;
            book.Genre = updatedBookDto.Genre;

            await _context.SaveChangesAsync();

            return new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Price = book.Price,
                PublishedDate = book.PublishedDate,
                Genre = book.Genre
            };
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Books.CountAsync();
        }

        // Borrowing functionality

        public async Task<BorrowDTO?> BorrowBookAsync(BorrowRequestDTO request)
        {
            var book = await _context.Books.FindAsync(request.BookId);
            if (book == null) return null;

            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null) return null;

            var borrow = new Borrow
            {
                BookId = request.BookId,
                UserId = request.UserId,
                BorrowDate = DateTime.Now
            };

            _context.Borrows.Add(borrow);
            await _context.SaveChangesAsync();

            return new BorrowDTO
            {
                BookId = borrow.BookId,
                BookTitle = book.Title,
                UserName = user.Name,
                BorrowDate = borrow.BorrowDate,
                ReturnDate = null
            };
        }

        public async Task<bool> ReturnBookAsync(int bookId, int userId)
        {
            var borrow = await _context.Borrows
                .FirstOrDefaultAsync(b => b.BookId == bookId && b.UserId == userId && b.ReturnDate == null);

            if (borrow == null) return false;

            borrow.ReturnDate = DateTime.Now;
            _context.Borrows.Update(borrow);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<BorrowDTO>> GetBorrowedBooksAsync(int userId)
        {
            return await _context.Borrows
                .Include(b => b.Book)
                .Where(b => b.UserId == userId && b.ReturnDate == null)
                .Select(b => new BorrowDTO
                {
                    BookId = b.BookId,
                    BookTitle = b.Book.Title,
                    UserName = b.User.Name,
                    BorrowDate = b.BorrowDate,
                    ReturnDate = b.ReturnDate
                })
                .ToListAsync();
        }

        public async Task<List<BorrowDTO>> GetAllBorrowedBooksAsync()
        {
            return await _context.Borrows
                .Include(b => b.Book)
                .Include(b => b.User)
                .Where(b => b.ReturnDate == null)
                .Select(b => new BorrowDTO
                {
                    BookId = b.BookId,
                    BookTitle = b.Book.Title,
                    UserName = b.User.Name,
                    BorrowDate = b.BorrowDate,
                    ReturnDate = b.ReturnDate
                })
                .ToListAsync();
        }
    }
}

