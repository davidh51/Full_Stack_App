using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helpers
{
    public class QueryObject
    {
        public string? Symbol { get; set; } = null;
        public string? CompanyName { get; set; } = null;
        public string? SortBy { get; set; } = null; //CompanyName or Symbol
        public bool IsDescending { get; set; } = false; //asc or desc
        public int PageNumber { get; set; } = 1; // Pagination
        public int PageSize { get; set; } = 10; // Pagination
    }
}