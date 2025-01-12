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
    [Authorize(Roles = "Admin")]
    public class BooksAdminController : Controller
    {
        private readonly HttpClient _httpClient;

        public BooksAdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        private void SetAuthorizationHeader()
        {
            var token = Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                TempData["ErrorMessage"] = "You are not authorized. Please log in.";
                RedirectToAction("Login", "Account");
            }
        }

        public async Task<IActionResult> Index(string? search = "", string sortBy = "Title", bool ascending = true, int page = 1, int pageSize = 10)
        {
            SetAuthorizationHeader();

            try
            {
                var booksResponse = await _httpClient.GetFromJsonAsync<PagedResponse<BookDTO>>(
                    $"api/Books?search={search}&sortBy={sortBy}&ascending={ascending}&pageNumber={page}&pageSize={pageSize}");

                if (booksResponse == null)
                {
                    TempData["ErrorMessage"] = "Failed to load books.";
                    return View(new BooksAdminViewModel());
                }

                var viewModel = new BooksAdminViewModel
                {
                    Books = booksResponse.Items,
                    SearchTerm = search,
                    SortBy = sortBy,
                    Ascending = ascending,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling((double)booksResponse.FilteredCount / pageSize)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return View(new BooksAdminViewModel());
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateOrUpdate(BookDTO book)
        {
            if (book.Id == 0)
            {
                // Create new book
                var response = await _httpClient.PostAsJsonAsync("api/Books", book);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Book created successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to create the book.";
                }
            }
            else
            {
                // Update existing book
                var response = await _httpClient.PutAsJsonAsync($"api/Books/{book.Id}", book);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Book updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update the book.";
                }
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            SetAuthorizationHeader();

            try
            {
                var response = await _httpClient.DeleteAsync($"api/Books/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Book deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete the book.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
