using Application.DTO.OrderDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Orders
{
    public class OrderWithUserInforDTO
    {
        public int? Id { get; set; }
        public decimal? OrderAmount { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? OrderStatus { get; set; }

        // Lấy thông tin từ OrderDetail đầu tiên
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public List<string> WarningMessages { get; set; } = new List<string>();
        public List<OrderDetailWithUserInfo> OrderDetails { get; set; } = new List<OrderDetailWithUserInfo>();
    }
}
