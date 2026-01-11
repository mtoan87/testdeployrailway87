using Application.Commons;
using Application.DTO.Dashboards;
using Application.DTO.Orders;
using Application.DTO.Products;
using Application.DTO.Records;
using Application.DTO.Users;
using Application.Interfaces;
using Application.IRepositories.Orders;
using Domain.Enum;
using Domain.Model;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories.Oders
{
    public class OrderRepo : GenericRepository<Order>, IOrderRepo
    {
        private readonly HypeCatDbContext _context;
        private readonly IClaimsService _claimsService;
        private readonly ICurrentTime _timeService;
        public OrderRepo(
            HypeCatDbContext context,            
            ICurrentTime timeService,
            IClaimsService claimsService
            ) :
            base(context, timeService, claimsService)
        {
            _context = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
        public async Task<List<MonthlyOrderStatsDTO>> GetMonthlyOrderStatsAsync(int year)
        {
            var result = await _context.Orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == year && o.OrderAmount.HasValue)
                .GroupBy(o => o.OrderDate.Value.Month)
                .Select(g => new MonthlyOrderStatsDTO
                {
                    Month = g.Key,
                    OrderCount = g.Count(),
                    TotalRevenue = g.Sum(o => o.OrderAmount.Value)
                })
                .ToListAsync();

            // Đảm bảo có đủ 12 tháng (nếu tháng nào không có đơn thì cho số 0)
            var fullYearStats = Enumerable.Range(1, 12).Select(month =>
            {
                var found = result.FirstOrDefault(r => r.Month == month);
                return new MonthlyOrderStatsDTO
                {
                    Month = month,
                    OrderCount = found?.OrderCount ?? 0,
                    TotalRevenue = found?.TotalRevenue ?? 0
                };
            }).ToList();

            return fullYearStats;
        }
        public async Task<List<RevenueByYearDTO>> GetRevenueByYearAsync()
        {
            var result = await _context.Orders
                .Where(o => o.OrderDate.HasValue
                            && o.OrderAmount.HasValue)                         
                .GroupBy(o => o.OrderDate.Value.Year)
                .Select(g => new RevenueByYearDTO
                {
                    Year = g.Key,
                    TotalRevenue = g.Sum(o => o.OrderAmount.Value)
                })
                .OrderBy(r => r.Year)
                .ToListAsync();

            return result;
        }
        public async Task<Pagination<Order>> GetPaginationAsync(OrderPaginationDTO paginationDTO)
        {
            var query = _context.Orders
            .Include(p => p.OrderDetails)
            .ThenInclude(od => od.Product)
            .ThenInclude(p => p.Logs)
            .Include(p => p.User)
            .AsQueryable();

            // Apply IsDeleted filter
            if (paginationDTO.IsDeleted.HasValue)
            {
                query = query.Where(p => p.IsDeleted == paginationDTO.IsDeleted);
            }

            // Apply search filters
            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                var searchTerm = paginationDTO.SearchTerm.ToLower();

                query = query.Where(p =>
                    p.User.Name.ToLower().Contains(searchTerm) || // User name
                    p.OrderDetails.Any(od =>
                        od.Name.ToLower().Contains(searchTerm) || // 🔍 Search in OrderDetail.Name
                        od.Product.Name.ToLower().Contains(searchTerm) // Product name
                    )
                );
            }


            if (paginationDTO.MinPrice.HasValue)
            {
                query = query.Where(p => p.OrderAmount >= paginationDTO.MinPrice);
            }

            if (paginationDTO.MaxPrice.HasValue)
            {
                query = query.Where(p => p.OrderAmount <= paginationDTO.MaxPrice);
            }

            if (!string.IsNullOrEmpty(paginationDTO.OrderStatus))
            {
                query = query.Where(p => p.OrderStatus == paginationDTO.OrderStatus);
            }

            

            // Apply sorting
            if (!string.IsNullOrEmpty(paginationDTO.SortBy))
            {
                query = paginationDTO.SortBy.ToLower() switch
                {
                    
                    "createdate" => paginationDTO.IsDescending ? query.OrderByDescending(p => p.CreateDate) : query.OrderBy(p => p.CreateDate),
                    "isdeleted" => paginationDTO.IsDescending ? query.OrderByDescending(p => p.IsDeleted) : query.OrderBy(p => p.IsDeleted),
                    _ => paginationDTO.IsDescending ? query.OrderByDescending(p => p.CreateDate) : query.OrderBy(p => p.CreateDate)
                };
            }
            else
            {
                query = paginationDTO.IsDescending ? query.OrderByDescending(p => p.CreateDate) : query.OrderBy(p => p.CreateDate);
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var items = await query
                .Skip(paginationDTO.PageIndex * paginationDTO.PageSize)
                .Take(paginationDTO.PageSize)
                .ToListAsync();

            return new Pagination<Order>
            {
                Items = items,
                TotalItemsCount = totalCount,
                PageSize = paginationDTO.PageSize,
                PageIndex = paginationDTO.PageIndex
            };
        }
        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Order>> GetOrdersByCurrentUserAsync()
        {
            var currentUserId = _claimsService.GetCurrentUserId;

            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.Logs)
                .Where(o => o.UserId == currentUserId)
                .ToListAsync();

            return orders;
        }
        public async Task<Order?> GetOrderWithOrderDetailsAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

           

            return order;
        }
        public async Task<int> GetTotalOrdersCountByYearAsync(int year)
        {
            var totalOrders = await _context.Orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == year)
                .CountAsync();

            return totalOrders;
        }
        public async Task<List<OrderDTO>> GetAllOrder()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return new List<OrderDTO>(); // Trả về danh sách rỗng nếu không có đơn hàng
            }

            var orderDTOs = orders.Select(order => new OrderDTO
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                OrderAmount = order.OrderAmount,
                OrderStatus = order.OrderStatus,

                User = order.User != null ? new UserOrderDetailDTO
                {
                    Name = order.User.Name,
                    Email = order.User.Email,
                    //Address = order.User.Address,
                    Phone = order.User.Phone
                } : null,

                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDTO
                {
                    Id = od.Id,
                    OrderId = od.OrderId,
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    TotalPrice = od.TotalPrice,

                    Product = od.Product != null ? new ProductOrderDTO
                    {
                        Id = od.Product.Id,
                        Name = od.Product.Name,
                        //SellingPrice = od.Product.SellingPrice
                    } : null

                }).ToList()

            }).ToList();

            return orderDTOs;
        }
        public List<OrderExportDto> orderExports(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var ordersQuery = _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .AsQueryable();

            // Lọc theo ngày nếu có truyền vào
            if (fromDate.HasValue && toDate.HasValue)
            {
                var from = fromDate.Value.Date;
                var to = toDate.Value.Date;
                ordersQuery = ordersQuery.Where(o => o.OrderDate.HasValue
                                                   && o.OrderDate.Value.Date >= from
                                                   && o.OrderDate.Value.Date <= to);
            }
            else if (fromDate.HasValue)
            {
                var dateOnly = fromDate.Value.Date;
                ordersQuery = ordersQuery.Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Date == dateOnly);
            }
            ordersQuery = ordersQuery.Where(o => o.OrderStatus == "Finish");
            var orders = ordersQuery.ToList();

            var exportData = new List<OrderExportDto>();

            foreach (var order in orders)
            {
                foreach (var detail in order.OrderDetails)
                {
                    var product = detail.Product;

                    exportData.Add(new OrderExportDto
                    {
                        CustomerName = detail.Name ?? "N/A",
                        ProductName = product?.Name ?? "Unknown",
                        Unit = product?.PacksPerUnit ?? 0,
                       // SourceOfProductName = product?.SourceofPro ?? "Unknown",
                        Quantity = detail.Quantity ?? 0,
                        UnitPrice = detail.UnitPrice ?? 0,
                        TotalPrice = detail.TotalPrice ?? ((detail.UnitPrice ?? 0) * (detail.Quantity ?? 0)),
                        OrderDate = detail.CreateDate,
                    });
                }
            }

            return exportData;
        }
        public async Task<OrderListDTO> GetOrderById(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return null;
            }

            var orderDTO = new OrderListDTO
            {

                Id = order.Id,
                OrderDate = order.OrderDate,
                OrderAmount = order.OrderAmount,
                PaymentMethod = order.PaymentMethod,
                OrderStatus = order.OrderStatus,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailss
                {
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    TotalPrice = od.TotalPrice,

                    Product = od.Product != null ? new ProductOrderDTO
                    {
                        Id = od.Product.Id,
                        Name = od.Product.Name,
                        //SellingPrice = od.Product.SellingPrice,
                        //StockQuantity = od.Product.StockQuantity,

                    } : null
                }).ToList(),
                Logs = new List<LogDTO>()
            };
            var firstDetail = order.OrderDetails.FirstOrDefault();
            if (firstDetail != null)
            {
                orderDTO.Name = firstDetail.Name;
                orderDTO.Phone = firstDetail.Phone;
                orderDTO.Address = firstDetail.Address;
            }
            // Lấy tất cả logs có liên quan tới đơn hàng
            var logs = await _context.Logs
                .Where(l => l.OrderId == orderId)
                .ToListAsync();

            orderDTO.Logs = logs.Select(log => new LogDTO
            {
                Id = log.Id,
                ProductId = log.ProductId,
                Quantity = log.Quantity,
                CreateDate = log.CreateDate,
                Type = log.Type
            }).ToList();

            return orderDTO;
        }
        public async Task<CheckOutDTO> CheckoutOrderAsync(int orderId, string paymentMethod)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Lấy thông tin đơn hàng kèm chi tiết và user
                            var order = await _context.Orders
                  .Include(o => o.OrderDetails)
                      .ThenInclude(od => od.Product)
                          .ThenInclude(p => p.BatchDetails)
                  .Include(o => o.User)
                  .FirstOrDefaultAsync(o => o.Id == orderId);
                if (order == null)
                    throw new ArgumentException("Đơn hàng không tồn tại!");

                if (order.OrderStatus == StatusOfOrder.Finish.ToString())
                    throw new InvalidOperationException("Đơn hàng đã được thanh toán!");

                if (order.OrderStatus != StatusOfOrder.Prepared.ToString())
                    throw new InvalidOperationException("Chỉ đơn hàng đã chuẩn bị mới được thanh toán!");

                // 2. Cập nhật trạng thái đơn hàng
                var orderDate = DateTime.Now;
                order.OrderStatus = StatusOfOrder.Finish.ToString();
                order.OrderDate = orderDate;
                order.PaymentMethod = paymentMethod;

                // 3. Ghi log thanh toán (Payment log, KHÔNG trừ kho nữa)
                var paymentLogs = order.OrderDetails.Select(od => new Log
                {
                    ProductId = od.ProductId,
                    OrderId = order.Id,
                    Name = od.Name,
                    Phone = od.Phone,
                    Address = od.Address,
                    Quantity = od.Quantity,
                    Type = LogType.Payment.ToString(),
                    CreateDate = orderDate,
                    Note = "Thanh toán đơn hàng"
                }).ToList();

                // 4. Cập nhật vào DB
                _context.Orders.Update(order);
                await _context.Logs.AddRangeAsync(paymentLogs);
                await _context.SaveChangesAsync();
                var orderstatus = new OrderStatus
                {
                    OrderId = order.Id,
                    AccountId = _claimsService.GetCurrentUserId,
                    Status = order.OrderStatus,                  
                    Note = "Đơn hàng đã được thanh toán",
                    CreateBy = _claimsService.GetCurrentUserName,
                    CreateDate = _timeService.GetCurrentTime()

                };
                await _context.OrderStatuses.AddAsync(orderstatus);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // 5. Trả về thông tin Checkout
                return new CheckOutDTO
                {
                    OrderDate = order.OrderDate,
                    OrderAmount = order.OrderAmount,
                    PaymentMethod = order.PaymentMethod,
                    OrderDetails = order.OrderDetails.Select(od => new CheckOutOrderDetailDTO
                    {
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice,
                        TotalPrice = od.TotalPrice,
                        Product = new ProductOrderDTO
                        {
                            Id = od.Product.Id,
                            Name = od.Product.Name,
                          
                        }
                    }).ToList()
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<List<OrderDetailDTO>> GetOrderDetailAsync(int orderId)
        {
            var orderDetails = await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .Include(od => od.Product)
                .Include(od => od.Order) // Chỉ Include Order
                .ToListAsync();

            // Lấy danh sách OrderId duy nhất từ OrderDetails
            var orderIds = orderDetails.Select(od => od.OrderId).Distinct().ToList();

            // Lấy danh sách Orders kèm theo User từ OrderIds
            var orders = await _context.Orders
                .Where(o => orderIds.Contains(o.Id))
                .Include(o => o.User) // Chỉ Include User tại đây
                .ToListAsync();

            var result = orderDetails.Select(od => new OrderDetailDTO
            {
                Id = od.Id,
                OrderId = od.OrderId,
                ProductId = od.ProductId,
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice,
                TotalPrice = od.TotalPrice,

                // Ánh xạ Product
                Product = od.Product != null ? new ProductOrderDTO
                {
                    Id = od.Product.Id,
                    Name = od.Product.Name,
                    //SellingPrice = od.Product.SellingPrice
                } : null,

                // Ánh xạ Order
                Order = orders.FirstOrDefault(o => o.Id == od.OrderId) != null ? new OrderDTO
                {
                    Id = od.Order.Id,
                    UserId = od.Order.UserId,
                    OrderAmount = od.Order.OrderAmount,
                    OrderDate = od.Order.OrderDate,
                    OrderStatus = od.Order.OrderStatus,

                    // Ánh xạ User
                    User = od.Order.User != null ? new UserOrderDetailDTO
                    {
                    
                        Name = od.Order.User.Name,
                        Email = od.Order.User.Email,
                        Phone = od.Order.User.Phone,
                        //Address = od.Order.User.Address,
                    } : null
                } : null

            }).ToList();

            return result;
        }
    }
}
