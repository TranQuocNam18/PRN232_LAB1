using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Models.Responses
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = null!;
        public PaginationMeta Pagination { get; set; } = null!;
    }

    public class PaginationMeta
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }

}
