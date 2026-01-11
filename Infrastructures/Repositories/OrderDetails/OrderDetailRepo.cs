using Application.DTO.Dashboards;
using Application.Interfaces;
using Application.IRepositories.OrderDetails;
using Application.IRepositories.Orders;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Application.DTO.OrderDetails;

namespace Infrastructures.Repositories.OrderDetails
{
    public class OrderDetailRepo : GenericRepository<OrderDetail>, IOrderDetailRepo
    {
        private readonly HypeCatDbContext _context;
        public OrderDetailRepo(
            HypeCatDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
            ) :
            base(context, timeService, claimsService)
        {
            _context = context;
        }
        public async Task<int> GetTotalSoldProductsAsync(int year)
        {
            // Lấy tất cả các OrderDetails có OrderDate trong năm
            var soldProducts = await _context.OrderDetails
                .Where(od => od.Order.OrderDate.HasValue && od.Order.OrderDate.Value.Year == year) // Kiểm tra năm của đơn hàng
                .GroupBy(od => od.ProductId) // Nhóm theo ProductId để tính mỗi sản phẩm duy nhất
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(od => od.Quantity ?? 0) // Cộng tổng số lượng của mỗi sản phẩm
                })
                .ToListAsync();

            // Trả về tổng số lượng sản phẩm bán được
            return soldProducts.Sum(sp => sp.TotalQuantity);
        }
        public async Task<List<TopCustomerFromOrderDetailDTO>> GetTop5CustomersFromOrderDetailAsync()
        {
            var topCustomers = await _context.OrderDetails
                .Where(od => !string.IsNullOrEmpty(od.Name) && od.TotalPrice.HasValue)
                .GroupBy(od => new { od.Name, od.Phone, od.Address, od.OrderId })
                .Select(g => new
                {
                    g.Key.Name,
                    g.Key.Phone,
                    g.Key.Address,
                    OrderId = g.Key.OrderId,
                    Total = g.Sum(x => x.TotalPrice ?? 0)
                })
                .GroupBy(x => new { x.Name, x.Phone, x.Address })
                .Select(g => new TopCustomerFromOrderDetailDTO
                {
                    Name = g.Key.Name,
                    Phone = g.Key.Phone,
                    Address = g.Key.Address,
                    OrderCount = g.Select(x => x.OrderId).Distinct().Count(),
                    TotalSpent = g.Sum(x => x.Total)
                })
                .OrderByDescending(c => c.OrderCount) // Hoặc .OrderByDescending(c => c.TotalSpent)
                .Take(5)
                .ToListAsync();

            return topCustomers;
        }


    }

}
