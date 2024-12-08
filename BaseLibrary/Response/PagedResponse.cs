using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BaseLibrary.Response
{
    public class PagedResponse<T>
    {
        [JsonPropertyName("books")] // Matches the "books" field in the API response
        public List<T> Items { get; set; } = new List<T>();

        [JsonPropertyName("totalCount")] // Matches the "totalCount" field in the API response
        public int TotalCount { get; set; }
        public int FilteredCount { get; set; } // Total books matching the search criteria
    }
}
