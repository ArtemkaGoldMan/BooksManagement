using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.DTOs
{
    public class BorrowDTO
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; } // Add this property to represent the user who borrowed the book
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
