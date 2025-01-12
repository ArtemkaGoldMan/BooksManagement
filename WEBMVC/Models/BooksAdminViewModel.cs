using BaseLibrary.DTOs;

namespace WEBMVC.Models
{
    public class BooksAdminViewModel
    {
        public List<BookDTO> Books { get; set; } = new();
        public string? SearchTerm { get; set; }
        public string SortBy { get; set; } = "Title";
        public bool Ascending { get; set; } = true;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
    }
}
