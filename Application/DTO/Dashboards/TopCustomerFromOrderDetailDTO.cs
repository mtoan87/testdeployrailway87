using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Dashboards
{
    public class TopCustomerFromOrderDetailDTO
    {
        public string Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
