using Application.Commons;
using Application.DTO.OrderDetails;
using Application.DTO.Orders;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Oders
{
    public interface IOrderService
    {
        Task<Pagination<OrderListDTO>> GetPaginationAsync(OrderPaginationDTO paginationDTO);
        Task<IEnumerable<OrderListDTO>> GetAllOrderAsync();
        Task<OrderNoLogDTO> CreateOrderAsync(CreateOrderDTO request);
        Task<OrderNoLogDTO> CreateOrderFromCartAsync(int addressId, string paymentmethod, string note);
        Task<List<OrderListDTO>> GetMyOrdersAsync();
        Task<IEnumerable<OrderListDTO>> GetOrderAsync();
        Task<OrderListDTO> GetOrderByIdAsync(int id);
        Task<List<OrderDetailDTO>> GetOrderDetailAsync(int orderId);
        Task<OrderWithUserInforDTO> UpdateOrderAsync(int id, UpdateOrderDTO accountDTO);
        Task<OrderNoLogDTO> UpdateOrderStatus(int id, string orderStatus);
        Task<CheckOutDTO> CheckOut(int orderId, string paymentMethod);
        List<OrderExportDto> orderExports(DateTime? fromDate = null, DateTime? toDate = null);
        
    }
}
