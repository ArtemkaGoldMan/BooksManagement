using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Services.Contracts;
using ServerLibrary.Services.Implementations;

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
        public async Task<IActionResult> CreateBook(CreateBookDto createBookDto)
        {
            var book = new Book
            {
                Title = createBookDto.Title,
                Author = createBookDto.Author,
                Price = createBookDto.Price,
                PublishedDate = createBookDto.PublishedDate,
                Genre = createBookDto.Genre
            };

            var createdBook = await _repository.AddBookAsync(book);
            return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, BookDto bookDto)
        {
            var book = new Book
            {
                Id = id,
                Title = bookDto.Title,
                Author = bookDto.Author,
                Price = bookDto.Price,
                PublishedDate = bookDto.PublishedDate,
                Genre = bookDto.Genre
            };

            var updatedBook = await _repository.UpdateBookAsync(id, book);
            if (updatedBook == null) return NotFound();

            return Ok(updatedBook);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await _repository.DeleteBookAsync(id);
            if (!result) return NotFound();

            return NoContent();
        }
    }
}
