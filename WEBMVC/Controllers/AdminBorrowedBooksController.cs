using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WEBMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminBorrowedBooksController : Controller
    {
        private readonly HttpClient _httpClient;

        public AdminBorrowedBooksController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        private void SetAuthorizationHeader()
        {
            var token = Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                TempData["ErrorMessage"] = "You are not authorized. Please log in.";
                RedirectToAction("Login", "Account");
            }
        }

        public async Task<IActionResult> Index(string filter = "all")
        {
            SetAuthorizationHeader();

            try
            {
                var response = await _httpClient.GetAsync("api/Books/history");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Failed to load borrowed books history.";
                    return View(new List<BorrowHistoryDTO>());
                }

                var borrowHistory = JsonConvert.DeserializeObject<List<BorrowHistoryDTO>>(await response.Content.ReadAsStringAsync()) ?? new List<BorrowHistoryDTO>();

                if (filter == "notReturned")
                {
                    borrowHistory = borrowHistory.Where(b => b.ReturnDate == null).ToList();
                }

                return View(borrowHistory);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return View(new List<BorrowHistoryDTO>());
            }
        }
    }
}
