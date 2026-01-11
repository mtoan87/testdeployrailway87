using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Batches
{
    public class BatchPaginationDTO
    {
        public string? SearchTerm { get; set; }      
        public bool? IsDeleted { get; set; }
        public int PageIndex { get; set; } = 0; // Thay đổi từ PageNumber thành PageIndex
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public bool IsDescending { get; set; } = false;
    }
}
