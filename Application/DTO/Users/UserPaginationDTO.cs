using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Users
{
    public class UserPaginationDTO
    {
        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
        public int? RoleId { get; set; }
        public bool? IsDeleted { get; set; }
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public bool IsDescending { get; set; } = false;
    }
}
