using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.Response
{
    public class PagedResponse<T>
    {
        public List<T> Items { get; set; }
        public int TotalPages { get; set; }
    }
}
