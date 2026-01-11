using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Dashboards
{
    public class DashboardDTO
    {
        
        public List<RevenueByYearDTO> RevenueByYears { get; set; }
        public List<MonthlyOrderStatsDTO> MonthlyOrderStats { get; set; }
        public List<TopProductDTO> TopSellingProducts { get; set; }
        public List<TopCustomerFromOrderDetailDTO> TopCustomers { get; set; }
        public CustomerStatsDTO CustomerStats { get; set; }
        public int TotalSoldProducts { get; set; }
        public int TotalOrders { get; set; }
    }
}
