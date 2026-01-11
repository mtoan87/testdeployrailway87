using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.News
{
    public class NewPaginationDTO
    {
        public string? SearchTerm { get; set; }
        public bool? IsDeleted { get; set; }
        public int PageIndex { get; set; } = 0; // Changed from PageNumber to PageIndex
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public bool IsDescending { get; set; } = false;
    }
}
