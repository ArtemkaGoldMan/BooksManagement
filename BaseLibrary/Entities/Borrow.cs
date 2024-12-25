using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.Entities
{
    public class Borrow
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; } // Navigation property
        public int UserId { get; set; }
        public User User { get; set; } // Navigation property
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; } // Nullable for ongoing borrows
    }
}
