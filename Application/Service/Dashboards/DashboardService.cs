using Application.DTO.Dashboards;
using Application.Interfaces;
using Application.Interfaces.Dashboards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Dashboards
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<List<TopProductDTO>> GetTop5BestSellingProductsAsync()
        {
            var result = await _unitOfWork.ProductRepo.GetTop5BestSellingProductsAsync();            
            return result;
        }

        public async Task<List<RevenueByYearDTO>> GetRevenueByYearAsync()
        {
            var rs = await _unitOfWork.OrderRepo.GetRevenueByYearAsync();
            return rs;
        }

        public async Task<List<MonthlyOrderStatsDTO>> GetMonthlyOrderStatsAsync(int year)
        {
            var rs = await _unitOfWork.OrderRepo.GetMonthlyOrderStatsAsync(year);
            return rs;
        }

        public async Task<List<TopCustomerFromOrderDetailDTO>> GetTop5CustomersFromOrderDetailAsync()
        {
            var rs = await _unitOfWork.OrderDetailRepo.GetTop5CustomersFromOrderDetailAsync();
            return rs;
        }

        public async Task<DashboardDTO> GetDashboardDataAsync(int year)
        {
            var revenueByYear = await _unitOfWork.OrderRepo.GetRevenueByYearAsync();
            var monthlyStats = await _unitOfWork.OrderRepo.GetMonthlyOrderStatsAsync(year);
            var topSellingProducts = await _unitOfWork.ProductRepo.GetTop5BestSellingProductsAsync();
            var topCustomers = await _unitOfWork.OrderDetailRepo.GetTop5CustomersFromOrderDetailAsync();
            var customerStats = await _unitOfWork.LogRepo.GetCustomerStatsAsync(); 
            var totalOrders = await _unitOfWork.OrderRepo.GetTotalOrdersCountByYearAsync(year);
            var totalSoldProducts = await _unitOfWork.OrderDetailRepo.GetTotalSoldProductsAsync(year);
            return new DashboardDTO
            {
                RevenueByYears = revenueByYear,
                MonthlyOrderStats = monthlyStats,
                TopSellingProducts = topSellingProducts,
                TopCustomers = topCustomers,
                CustomerStats = customerStats,
                TotalOrders = totalOrders,
                TotalSoldProducts = totalSoldProducts
            };
        }

    }
}
