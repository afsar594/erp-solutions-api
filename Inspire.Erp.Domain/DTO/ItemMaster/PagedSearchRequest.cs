using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Domain.DTO.ItemMaster
{
    public class PagedSearchRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string SearchTerm { get; set; }
        public string SortColumn { get; set; } = "Id";
        public string SortDirection { get; set; } = "asc";
    }
}
