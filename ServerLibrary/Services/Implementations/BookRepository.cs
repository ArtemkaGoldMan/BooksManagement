﻿using BaseLibrary.Entities;
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

        public async Task<PagedResponse<Book>> GetBooksAsync(string? search, string? sortBy, bool ascending, int pageNumber, int pageSize)
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
                .ToListAsync();

            // Return both items and counts
            return new PagedResponse<Book>
            {
                Items = items,
                TotalCount = totalCount,
                FilteredCount = filteredCount
            };
        }


        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<Book> AddBookAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Book?> UpdateBookAsync(int id, Book updatedBook)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return null;

            book.Title = updatedBook.Title;
            book.Author = updatedBook.Author;
            book.Price = updatedBook.Price;
            book.PublishedDate = updatedBook.PublishedDate;
            book.Genre = updatedBook.Genre;

            await _context.SaveChangesAsync();
            return book;
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
    }
}
