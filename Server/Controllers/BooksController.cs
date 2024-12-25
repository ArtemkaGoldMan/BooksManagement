using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Services.Contracts;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _repository;

        public BooksController(IBookRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks([FromQuery] string? search, [FromQuery] string? sortBy, [FromQuery] bool ascending = true, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var response = await _repository.GetBooksAsync(search, sortBy, ascending, pageNumber, pageSize);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _repository.GetBookByIdAsync(id);
            if (book == null) return NotFound();

            return Ok(book);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBook(CreateBookDTO createBookDto)
        {
            var createdBook = await _repository.AddBookAsync(createBookDto);
            return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBook(int id, UpdateBookDTO updateBookDto)
        {
            var updatedBook = await _repository.UpdateBookAsync(id, updateBookDto);
            if (updatedBook == null) return NotFound();

            return Ok(updatedBook);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await _repository.DeleteBookAsync(id);
            if (!result) return NotFound();

            return NoContent();
        }

        [HttpPost("borrow")]
        public async Task<IActionResult> BorrowBook(BorrowRequestDTO request)
        {
            var borrow = await _repository.BorrowBookAsync(request);
            if (borrow == null) return BadRequest("Unable to borrow book. Ensure the book and user exist and are eligible.");

            return Ok(borrow);
        }

        [HttpPost("return")]
        public async Task<IActionResult> ReturnBook([FromQuery] int bookId, [FromQuery] int userId)
        {
            var result = await _repository.ReturnBookAsync(bookId, userId);
            if (!result) return BadRequest("Unable to return book. Ensure the book and user exist, and the book was borrowed by this user.");

            return Ok("Book returned successfully.");
        }

        [HttpGet("borrowed/{userId}")]
        public async Task<IActionResult> GetBorrowedBooks(int userId)
        {
            var borrows = await _repository.GetBorrowedBooksAsync(userId);
            return Ok(borrows);
        }

        [HttpGet("borrowed")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBorrowedBooks()
        {
            var borrows = await _repository.GetAllBorrowedBooksAsync();
            return Ok(borrows);
        }
    }
}
