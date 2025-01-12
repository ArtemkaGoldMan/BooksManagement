using BaseLibrary.DTOs;

namespace WEBMVC.Models
{
    public class BooksViewModel
    {
        public List<BookDTO> Books { get; set; } = new List<BookDTO>();
        public List<BorrowDTO> BorrowedBooks { get; set; } = new List<BorrowDTO>();
        public List<BorrowDTO> AllBorrowedBooks { get; set; } = new List<BorrowDTO>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public string SearchTerm { get; set; } = string.Empty;
        public string SortBy { get; set; } = "Title"; // Default sort column
        public bool Ascending { get; set; } = true; // Default sort direction (ascending)
        public bool ShowBorrowedByUser { get; set; } = false;
    }
}

