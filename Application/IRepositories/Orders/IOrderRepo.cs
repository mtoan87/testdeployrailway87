using Application.Commons;
using Application.DTO.Dashboards;
using Application.DTO.Orders;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories.Orders
{
    public interface IOrderRepo : IGenericRepository<Order>
    {
        Task<Order?> GetOrderWithOrderDetailsAsync(int id);
        Task<OrderListDTO> GetOrderById(int orderId);
        Task<Order> GetOrderByIdAsync(int id);
        Task<List<Order>> GetOrdersByCurrentUserAsync();
        Task<int> GetTotalOrdersCountByYearAsync(int year);
        Task<List<MonthlyOrderStatsDTO>> GetMonthlyOrderStatsAsync(int year);
        Task<List<RevenueByYearDTO>> GetRevenueByYearAsync();
        Task<List<OrderDTO>> GetAllOrder();
        Task<Pagination<Order>> GetPaginationAsync(OrderPaginationDTO paginationDTO);
        Task<List<OrderDetailDTO>> GetOrderDetailAsync(int orderId);
        Task<CheckOutDTO> CheckoutOrderAsync(int orderId, string paymentMethod);
        List<OrderExportDto> orderExports(DateTime? fromDate = null, DateTime? toDate = null);
    }
}
