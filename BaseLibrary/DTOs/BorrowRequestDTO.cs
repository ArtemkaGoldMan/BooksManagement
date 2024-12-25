using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.DTOs
{
    public class BorrowRequestDTO
    {
        public int BookId { get; set; }
        public int UserId { get; set; }
    }
}
