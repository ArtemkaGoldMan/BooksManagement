using BaseLibrary.DTOs;
using BaseLibrary.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using WEBMVC.Models;

namespace WEBMVC.Controllers
{
    [Authorize] // Ensure only authenticated users can access
    public class BooksController : Controller
    {
        private readonly HttpClient _httpClient;

        public BooksController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        private void SetAuthorizationHeader()
        {
            var token = Request.Cookies["jwt"]; // Retrieve the token from cookies
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                // Redirect to login if the token is missing
                TempData["ErrorMessage"] = "You are not authorized. Please log in.";
                RedirectToAction("Login", "Account");
            }
        }

        public async Task<IActionResult> Index(
                                            string? search = "",
                                            string sortBy = "Title",
                                            bool ascending = true,
                                            int page = 1,
                                            int pageSize = 10,
                                            bool showBorrowedByUser = false)
        {
            SetAuthorizationHeader(); // Ensure the token is set

            try
            {
                List<BookDTO> books = new();
                List<BorrowDTO> borrowedBooks = new();
                List<BorrowDTO> allBorrowedBooks = new();
                int totalItems = 0;

                if (showBorrowedByUser)
                {
                    // Show only books borrowed by the current user
                    var borrowedBooksResponse = await _httpClient.GetAsync($"api/Books/borrowed/{User.FindFirst("UserId")?.Value}");
                    if (borrowedBooksResponse.IsSuccessStatusCode)
                    {
                        borrowedBooks = JsonConvert.DeserializeObject<List<BorrowDTO>>(await borrowedBooksResponse.Content.ReadAsStringAsync()) ?? new List<BorrowDTO>();
                        books = borrowedBooks.Select(b => new BookDTO
                        {
                            Id = b.BookId,
                            Title = b.BookTitle,
                            Author = b.UserName, // Assuming UserName is stored here
                            Genre = "N/A", // Genre is not relevant for borrowed books
                            Price = 0 // Price is not relevant for borrowed books
                        }).ToList();
                        totalItems = books.Count;
                    }
                }
                else
                {
                    // Show all books with pagination
                    var booksResponse = await _httpClient.GetFromJsonAsync<PagedResponse<BookDTO>>(
                        $"api/Books?search={search}&sortBy={sortBy}&ascending={ascending}&pageNumber={page}&pageSize={pageSize}");

                    if (booksResponse != null)
                    {
                        books = booksResponse.Items;
                        totalItems = booksResponse.FilteredCount;

                        var allBorrowedBooksResponse = await _httpClient.GetAsync("api/Books/borrowed");
                        if (allBorrowedBooksResponse.IsSuccessStatusCode)
                        {
                            allBorrowedBooks = JsonConvert.DeserializeObject<List<BorrowDTO>>(await allBorrowedBooksResponse.Content.ReadAsStringAsync()) ?? new List<BorrowDTO>();
                        }
                    }
                }

                var viewModel = new BooksViewModel
                {
                    Books = books,
                    BorrowedBooks = borrowedBooks,
                    AllBorrowedBooks = allBorrowedBooks,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                    SearchTerm = search,
                    SortBy = sortBy,
                    Ascending = ascending,
                    ShowBorrowedByUser = showBorrowedByUser
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return View(new BooksViewModel());
            }
        }




        [HttpPost]
        public async Task<IActionResult> Borrow(int bookId)
        {
            SetAuthorizationHeader();

            var borrowRequest = new BorrowRequestDTO
            {
                BookId = bookId,
                UserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0")
            };

            var content = new StringContent(JsonConvert.SerializeObject(borrowRequest), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Books/borrow", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Book borrowed successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to borrow the book. It might already be borrowed.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Return(int bookId)
        {
            SetAuthorizationHeader();

            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var response = await _httpClient.PostAsync($"api/Books/return?bookId={bookId}&userId={userId}", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Book returned successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to return the book.";
            }

            return RedirectToAction("Index");
        }
    }
}
