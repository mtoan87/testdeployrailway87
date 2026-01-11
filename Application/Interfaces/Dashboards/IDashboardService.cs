using Application.DTO.Dashboards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Dashboards
{
    public interface IDashboardService
    {
        Task<DashboardDTO> GetDashboardDataAsync(int year);
        Task<List<TopCustomerFromOrderDetailDTO>> GetTop5CustomersFromOrderDetailAsync();
        Task<List<TopProductDTO>> GetTop5BestSellingProductsAsync();
        Task<List<MonthlyOrderStatsDTO>> GetMonthlyOrderStatsAsync(int year);
        Task<List<RevenueByYearDTO>> GetRevenueByYearAsync();
    }
}
