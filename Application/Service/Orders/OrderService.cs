using Application.Commons;
using Application.DTO.BatchDetails;
using Application.DTO.OrderDetails;
using Application.DTO.Orders;
using Application.DTO.Products;
using Application.DTO.Records;
using Application.DTO.Users;
using Application.Interfaces;
using Application.Interfaces.Oders;
using Application.IRepositories.Orders;
using Application.IRepositories.Products;
using Application.Service.Products;
using Application.Utils;
using CloudinaryDotNet.Core;
using Domain.Enum;
using Domain.Model;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using PdfSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace Application.Service.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;       
        private readonly IClaimsService _claimsService;

        public OrderService(IUnitOfWork unitOfWork, IClaimsService claimsService)
        {                     
            _unitOfWork = unitOfWork;
            _claimsService = claimsService;
        }
        public async Task<Pagination<OrderListDTO>> GetPaginationAsync(OrderPaginationDTO paginationDTO)
        {
            try
            {
                var pagination = await _unitOfWork.OrderRepo.GetPaginationAsync(paginationDTO);

                // Map danh sách đơn hàng sang DTO
                var orderDTOs = pagination.Items.Adapt<List<OrderListDTO>>();

                // Lấy danh sách tất cả orderId đang hiển thị
                var orderIds = pagination.Items.Select(o => o.Id).ToList();

                // Lấy toàn bộ Logs liên quan đến các đơn hàng này (chỉ "Export")
                var allLogs = await _unitOfWork.LogRepo.GetLogsByOrderIdsAsync(orderIds);

                // Gán logs tương ứng vào từng orderDTO và chỉ hiển thị thông tin người dùng một lần
                foreach (var orderDto in orderDTOs)
                {
                    var orderEntity = pagination.Items.First(o => o.Id == orderDto.Id);

                    // Thông tin người dùng, chỉ lấy một lần cho tất cả các OrderDetails
                    var firstDetail = orderEntity.OrderDetails.FirstOrDefault();
                    if (firstDetail != null)
                    {
                        orderDto.Name = firstDetail.Name;
                        orderDto.Address = firstDetail.Address;
                        orderDto.Phone = firstDetail.Phone;
                       
                    }
                    // Ánh xạ OrderDetails
                    orderDto.OrderDetails = orderEntity.OrderDetails.Select(od => new OrderDetailss
                    {
                        ProductId = od.ProductId,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice,
                        TotalPrice = od.TotalPrice,
                        Product = od.Product != null ? new ProductOrderDTO
                        {
                            Id = od.Product.Id,
                            Name = od.Product.Name,
                            
                        } : null
                    }).ToList();

                    // Lấy Logs cho từng orderDto
                    var logs = allLogs
                        .Where(log => log.OrderId == orderDto.Id)
                        .Select(log => new LogDTO
                        {
                            Id = log.Id,
                            ProductId = log.ProductId,
                            Quantity = log.Quantity,
                            Type = log.Type,
                            CreateDate = log.CreateDate,
                        })
                        .ToList();

                    orderDto.Logs = logs;
                }

                return new Pagination<OrderListDTO>
                {
                    Items = orderDTOs,
                    TotalItemsCount = pagination.TotalItemsCount,
                    PageSize = pagination.PageSize,
                    PageIndex = pagination.PageIndex
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting pagination: {ex.Message}", ex);
            }
        }
        public async Task<IEnumerable<OrderListDTO>> GetOrderAsync()
        {
            var accounts = await _unitOfWork.OrderRepo.GetAllAsync(includeProperties: "User,OrderDetails");
            return accounts.Adapt<IEnumerable<OrderListDTO>>();
        }
        public async Task<IEnumerable<OrderListDTO>> GetAllOrderAsync()
        {
            var accounts = await _unitOfWork.OrderRepo.GetAllOrder();
            return accounts.Adapt<IEnumerable<OrderListDTO>>();
        }
        public async Task<List<OrderDetailDTO>> GetOrderDetailAsync(int orderId) => await _unitOfWork.OrderRepo.GetOrderDetailAsync(orderId);
        public async Task<OrderListDTO> GetOrderByIdAsync(int id)
        {
            var exist = await _unitOfWork.OrderRepo.GetOrderById(id);
            if (exist == null)
            {
                throw new Exception("Order is not existed");
            }
            return exist.Adapt<OrderListDTO>();
        }

        public async Task<List<OrderListDTO>> GetMyOrdersAsync()
        {
            var userOrders = await _unitOfWork.OrderRepo.GetOrdersByCurrentUserAsync();
            return userOrders.Adapt<List<OrderListDTO>>();
        }
        public async Task<CheckOutDTO> CheckOut(int orderId, string paymentMethod) => await _unitOfWork.OrderRepo.CheckoutOrderAsync(orderId, paymentMethod);
        public async Task<OrderNoLogDTO> CreateOrderAsync(CreateOrderDTO request)
        {
            if (request.OrderDetails == null || !request.OrderDetails.Any())
                throw new Exception("Order must have at least one product.");

            var orderDate = DateTime.Now;

            var productIds = request.OrderDetails.Select(d => d.ProductId).ToList();
            var products = await _unitOfWork.ProductRepo.GetByIdsAsync(productIds);
            var batchDetails = await _unitOfWork.BatchDetailRepo.GetByProductIdsAsync(productIds);

            var order = new Order
            {
                OrderDate = orderDate,
                OrderStatus = StatusOfOrder.Pending.ToString(),
                OrderAmount = 0
            };

            await _unitOfWork.OrderRepo.AddAsync(order);
            await _unitOfWork.SaveChangeAsync();

            decimal? totalAmount = 0;
            var orderDetails = new List<OrderDetail>();
            var orderDetailsDTO = new List<OrderDetailss>();
            var warningMessages = new List<string>();

            foreach (var detail in request.OrderDetails)
            {
                var product = products.FirstOrDefault(p => p.Id == detail.ProductId);
                if (product == null) continue;

                var productBatches = batchDetails
                    .Where(b => b.ProductId == detail.ProductId && (b.Quantity ?? 0) > 0)
                    .OrderBy(b => b.CreateDate)
                    .ToList();

                int neededQty = detail.Quantity;
                decimal totalDetailPrice = 0;
                var batchBreakdown = new List<BatchUsedDTO>();

                foreach (var batch in productBatches)
                {
                    if (neededQty <= 0) break;

                    int available = batch.RemainingQuantity ?? 0;
                    int used = Math.Min(available, neededQty);
                    if (used <= 0) continue;

                    decimal unitPrice = batch.SellingPrice ?? 0;
                    totalDetailPrice += used * unitPrice;

                    //batch.RemainingQuantity -= used;
                    //_unitOfWork.BatchDetailRepo.Update(batch);

                    neededQty -= used;

                    batchBreakdown.Add(new BatchUsedDTO
                    {
                        BatchDetailId = batch.Id,
                        QuantityUsed = used,
                        SellingPrice = unitPrice
                    });
                }

                if (neededQty > 0)
                {
                    warningMessages.Add($"⚠️ Sản phẩm [{product.Name}] chỉ có thể xuất [{detail.Quantity - neededQty}/{detail.Quantity}].");
                }

                var orderDetail = new OrderDetail
                {
                    ProductId = detail.ProductId,
                    OrderId = order.Id,
                    Quantity = detail.Quantity,
                    Name = request.Name,
                    Phone = request.Phone,
                    Address = request.Address,
                    Email = request.Email,
                    UnitPrice = detail.Quantity > 0 ? totalDetailPrice / detail.Quantity : 0,
                    TotalPrice = totalDetailPrice
                };

                totalAmount += orderDetail.TotalPrice;
                orderDetails.Add(orderDetail);

                orderDetailsDTO.Add(new OrderDetailss
                {
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                    UnitPrice = orderDetail.UnitPrice,
                    TotalPrice = totalDetailPrice,
                    BatchBreakdown = batchBreakdown
                });
            }

            await _unitOfWork.OrderDetailRepo.AddRangeAsync(orderDetails);

            order.OrderAmount = totalAmount;
            _unitOfWork.OrderRepo.Update(order);
            await _unitOfWork.SaveChangeAsync();

            var orderDto = new OrderNoLogDTO
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                OrderAmount = order.OrderAmount,
                Name = request.Name,
                Phone = request.Phone,
                Address = request.Address,
                Message = warningMessages.Any()
                    ? string.Join(" | ", warningMessages)
                    : "Đơn hàng đã được tạo thành công.",
                OrderDetails = orderDetailsDTO
            };

            return orderDto;
        }
        public async Task<OrderWithUserInforDTO> UpdateOrderAsync(int id, UpdateOrderDTO updateDTO)
        {
            try
            {
                var warningMessages = new List<string>();

                var existingOrder = await _unitOfWork.OrderRepo.GetOrderWithOrderDetailsAsync(id);
                if (existingOrder == null)
                    throw new Exception("Order does not exist.");

                if (!existingOrder.OrderStatus?.Equals(StatusOfOrder.Pending.ToString(), StringComparison.OrdinalIgnoreCase) == true)
                    throw new Exception("Only orders with 'Pending' status can be updated.");

                if (string.IsNullOrWhiteSpace(updateDTO.Name) ||
                    string.IsNullOrWhiteSpace(updateDTO.Phone) ||
                    string.IsNullOrWhiteSpace(updateDTO.Address))
                    throw new Exception("Name, Phone, and Address are required.");

                // --- Xóa sản phẩm ---
                if (updateDTO.DeletedProductIds != null && updateDTO.DeletedProductIds.Any())
                {
                    foreach (var deletedProductId in updateDTO.DeletedProductIds)
                    {
                        var orderDetail = existingOrder.OrderDetails.FirstOrDefault(od => od.ProductId == deletedProductId);
                        if (orderDetail == null)
                            throw new Exception($"Cannot delete. ProductId {deletedProductId} does not exist in this Order.");

                        await _unitOfWork.OrderDetailRepo.DeleteAsync(orderDetail);
                        existingOrder.OrderDetails.Remove(orderDetail); // ⚠️ Bắt buộc phải loại khỏi danh sách hiện tại
                    }
                }

                // --- Thêm hoặc cập nhật sản phẩm trong đơn ---
                if (updateDTO.OrderDetails != null && updateDTO.OrderDetails.Any())
                {
                    var productIds = updateDTO.OrderDetails.Select(d => d.ProductId).ToList();
                    var products = await _unitOfWork.ProductRepo.GetByIdsAsync(productIds);
                    var batchDetails = await _unitOfWork.BatchDetailRepo.GetByProductIdsAsync(productIds);

                    foreach (var detail in updateDTO.OrderDetails)
                    {
                        if (detail.Quantity == null || detail.Quantity <= 0)
                            throw new Exception($"Quantity for ProductId {detail.ProductId} must be greater than 0.");

                        var product = products.FirstOrDefault(p => p.Id == detail.ProductId);
                        if (product == null)
                            throw new Exception($"Product with ID {detail.ProductId} does not exist.");

                        var productBatches = batchDetails
                            .Where(b => b.ProductId == detail.ProductId && (b.Quantity ?? 0) > 0)
                            .OrderBy(b => b.ExpiredDate)
                            .ToList();

                        int neededQty = detail.Quantity.Value;
                        decimal totalDetailPrice = 0;

                        foreach (var batch in productBatches)
                        {
                            if (neededQty <= 0) break;

                            int available = batch.Quantity ?? 0;
                            int used = Math.Min(available, neededQty);

                            decimal unitPrice = batch.SellingPrice ?? 0;
                            totalDetailPrice += used * unitPrice;

                            batch.Quantity -= used;
                            _unitOfWork.BatchDetailRepo.Update(batch);

                            neededQty -= used;
                        }

                        if (neededQty > 0)
                        {
                            warningMessages.Add($"⚠️ Sản phẩm [{product.Name}] chỉ có thể xuất [{detail.Quantity.Value - neededQty}/{detail.Quantity.Value}].");
                        }

                        var existingDetail = existingOrder.OrderDetails.FirstOrDefault(od => od.ProductId == detail.ProductId);
                        if (existingDetail != null)
                        {
                            existingDetail.Quantity = detail.Quantity;
                            existingDetail.UnitPrice = totalDetailPrice / detail.Quantity.Value;
                            existingDetail.TotalPrice = totalDetailPrice;
                            existingDetail.Name = updateDTO.Name;
                            existingDetail.Phone = updateDTO.Phone;
                            existingDetail.Address = updateDTO.Address;
                        }
                        else
                        {
                            var newDetail = new OrderDetail
                            {
                                OrderId = existingOrder.Id,
                                ProductId = detail.ProductId,
                                Quantity = detail.Quantity,
                                UnitPrice = totalDetailPrice / detail.Quantity.Value,
                                TotalPrice = totalDetailPrice,
                                Name = updateDTO.Name,
                                Phone = updateDTO.Phone,
                                Address = updateDTO.Address
                            };

                            await _unitOfWork.OrderDetailRepo.AddAsync(newDetail);
                        }
                    }
                }

                // --- Nếu không có OrderDetails truyền lên thì chỉ cập nhật thông tin người nhận ---
                if ((updateDTO.OrderDetails == null || !updateDTO.OrderDetails.Any()) &&
                    (updateDTO.DeletedProductIds == null || !updateDTO.DeletedProductIds.Any()))
                {
                    foreach (var orderDetail in existingOrder.OrderDetails)
                    {
                        orderDetail.Name = updateDTO.Name;
                        orderDetail.Phone = updateDTO.Phone;
                        orderDetail.Address = updateDTO.Address;
                    }
                }

                // --- Cập nhật tổng tiền ---
                existingOrder.OrderAmount = existingOrder.OrderDetails.Sum(od => od.TotalPrice ?? 0);

                _unitOfWork.OrderRepo.Update(existingOrder);
                await _unitOfWork.SaveChangeAsync();

                var firstOrderDetail = existingOrder.OrderDetails.FirstOrDefault();

                var result = new OrderWithUserInforDTO
                {
                    Id = existingOrder.Id,
                    OrderAmount = existingOrder.OrderAmount,
                    OrderDate = existingOrder.OrderDate,
                    OrderStatus = existingOrder.OrderStatus,
                    Name = firstOrderDetail?.Name,
                    Phone = firstOrderDetail?.Phone,
                    Address = firstOrderDetail?.Address,
                    WarningMessages = warningMessages,
                    OrderDetails = existingOrder.OrderDetails.Select(od => new OrderDetailWithUserInfo
                    {
                        ProductId = od.ProductId,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice,
                        TotalPrice = od.TotalPrice,
                        Product = od.Product != null ? new ProductOrderDTO
                        {
                            Id = od.Product.Id,
                            Name = od.Product.Name,
                        } : null
                    }).ToList()
                };

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Có lỗi xảy ra khi cập nhật đơn hàng: " + ex.Message);
                throw;
            }
        }
        public async Task<OrderNoLogDTO> UpdateOrderStatus(int id, string orderStatus)
        {
            var existingOrder = await _unitOfWork.OrderRepo.GetOrderByIdAsync(id);

            if (existingOrder == null)
            {
                throw new Exception("Đơn hàng không tồn tại!");
            }

            if (orderStatus == "Finish")
            {
                throw new Exception("Có lỗi xảy ra khi cập nhật trạng thái của đơn hàng!");
            }

            if (existingOrder.OrderStatus == "Finish")
            {
                throw new Exception("Không thể thanh toán đơn hàng đã thanh toán!");
            }

            // === Trường hợp từ Pending → Prepared ===
            if (orderStatus == "Prepared")
            {
                var outOfStockMessages = new List<string>();
                var productIds = existingOrder.OrderDetails.Select(d => d.ProductId.Value).ToList();
                var batchDetails = await _unitOfWork.BatchDetailRepo.GetByProductIdsAsync(productIds);
                var exportLogs = new List<Log>();

                // --- Kiểm tra tồn kho ---
                foreach (var detail in existingOrder.OrderDetails)
                {
                    var productBatches = batchDetails.Where(b => b.ProductId == detail.ProductId).ToList();
                    int totalAvailableQuantity = productBatches.Sum(b => b.RemainingQuantity ?? 0);

                    if (totalAvailableQuantity < detail.Quantity)
                    {
                        string name = detail.Product?.Name ?? "Unknown";
                        outOfStockMessages.Add($"[Mã sản phẩm: {detail.ProductId}, Tên sản phẩm: {name}, Tồn kho: {totalAvailableQuantity}, Mua: {detail.Quantity}]");
                    }
                }

                if (outOfStockMessages.Any())
                {
                    throw new Exception("Không thể chuẩn bị đơn hàng vì không đủ số lượng tồn kho: " +
                                         string.Join(", ", outOfStockMessages));
                }

                // --- Trừ tồn và ghi log ---
                foreach (var detail in existingOrder.OrderDetails)
                {
                    var productBatches = batchDetails
                        .Where(b => b.ProductId == detail.ProductId)
                        .OrderBy(b => b.Id)
                        .ToList();

                    int remainingQuantity = detail.Quantity ?? 0;

                    foreach (var batch in productBatches)
                    {
                        if (remainingQuantity <= 0)
                            break;

                        int currentQty = batch.RemainingQuantity ?? 0;

                        // Nếu batch này hết hàng → bỏ qua
                        if (currentQty <= 0)
                            continue;

                        // Tính lượng thực tế xuất
                        int usedQty = Math.Min(currentQty, remainingQuantity);
                        if (usedQty <= 0)
                            continue;

                        // Trừ kho
                        batch.RemainingQuantity = currentQty - usedQty;
                        _unitOfWork.BatchDetailRepo.Update(batch);

                        // ✅ Ghi log CHỈ khi có trừ thật
                        exportLogs.Add(new Log
                        {
                            OrderId = existingOrder.Id,
                            ProductId = detail.ProductId,
                            Quantity = usedQty,
                            Type = "Export",
                            BatchId = batch.BatchId,
                            BatchDetailId = batch.Id,
                            CreateDate = DateTime.Now,
                            Note = "Chuẩn bị đơn hàng"
                        });

                        remainingQuantity -= usedQty;
                    }
                }

                await _unitOfWork.LogRepo.AddRangeAsync(exportLogs);
                await _unitOfWork.SaveChangeAsync();

                // Ghi trạng thái đơn hàng
                var orderstatus = new OrderStatus
                {
                    OrderId = existingOrder.Id,
                    Status = orderStatus,
                    AccountId = _claimsService.GetCurrentUserId,
                    Note = "Đơn hàng đã được chuẩn bị"
                };
                await _unitOfWork.OrderStatusRepo.AddAsync(orderstatus);
                await _unitOfWork.SaveChangeAsync();
            }

            // === Trường hợp từ Prepared → Canceled (Rollback kho + log) ===
            if (existingOrder.OrderStatus == "Prepared" && orderStatus == "Canceled")
            {
                var rollbackLogs = new List<Log>();

                foreach (var detail in existingOrder.OrderDetails)
                {
                    var exportLogs = await _unitOfWork.LogRepo.GetLogsByOrderAndProductAsync(existingOrder.Id, detail.ProductId.Value, "Export");

                    foreach (var exportLog in exportLogs)
                    {
                        var batch = await _unitOfWork.BatchDetailRepo.GetByIdAsync(exportLog.BatchDetailId ?? 0);
                        if (batch != null)
                        {
                            batch.RemainingQuantity = (batch.RemainingQuantity ?? 0) + exportLog.Quantity;
                            _unitOfWork.BatchDetailRepo.Update(batch);

                            rollbackLogs.Add(new Log
                            {
                                OrderId = existingOrder.Id,
                                ProductId = detail.ProductId,
                                Quantity = exportLog.Quantity,
                                Type = "Rollback",
                                CreateDate = DateTime.Now,
                                BatchDetailId = batch.Id,
                                Note = "Hoàn kho do huỷ đơn hàng"
                            });
                        }
                    }
                }

                await _unitOfWork.LogRepo.AddRangeAsync(rollbackLogs);
                await _unitOfWork.SaveChangeAsync(); // ✅ Save sau khi hoàn kho và ghi log

                var orderstatus = new OrderStatus
                {
                    OrderId = existingOrder.Id,
                    Status = orderStatus,
                    AccountId = _claimsService.GetCurrentUserId,
                    Note = "Đơn hàng đã bị huỷ"
                };
                await _unitOfWork.OrderStatusRepo.AddAsync(orderstatus);
                await _unitOfWork.SaveChangeAsync(); // ✅ Save trạng thái đơn hàng
            }

            existingOrder.OrderStatus = orderStatus;
            _unitOfWork.OrderRepo.Update(existingOrder);
            await _unitOfWork.SaveChangeAsync(); // ✅ Save trạng thái đơn hàng

            var result = existingOrder.Adapt<OrderNoLogDTO>();
            result.Message = orderStatus == "Prepared"
                ? "Đơn hàng đã được chuẩn bị."
                : $"Trạng thái đơn hàng đã được cập nhật thành: {orderStatus}";

            return result;
        }
        public List<OrderExportDto> orderExports(DateTime? fromDate = null, DateTime? toDate = null)
        {
            return _unitOfWork.OrderRepo.orderExports(fromDate, toDate);
        }
        public async Task<OrderNoLogDTO> CreateOrderFromCartAsync(int addressId ,string paymentmethod, string note)
        {
            var userId = _claimsService.GetCurrentUserId;
            var user = await _unitOfWork.UserRepo.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("Không tìm thấy người dùng!");
            var address = await _unitOfWork.AddressRepo.GetAddressByAddressIdAndUserId(addressId);
            if(address == null)
            {
                throw new Exception("Không tìm thấy địa chỉ!");
            }
            var cartItems = await _unitOfWork.CartRepo.GetUserCartByUserId();

            if (cartItems == null || !cartItems.Any())
                throw new Exception("Giỏ hàng trống!");

            var productIds = cartItems.Select(c => c.ProductId).Distinct().ToList();
            var products = await _unitOfWork.ProductRepo.GetByIdsAsync(productIds);
            var batchDetails = await _unitOfWork.BatchDetailRepo.GetByProductIdsAsync(productIds);

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                OrderStatus = StatusOfOrder.Pending.ToString(),
                OrderAmount = 0,
                AddressId = addressId,  
                PaymentMethod = paymentmethod,
                Note = note,


            };

            await _unitOfWork.OrderRepo.AddAsync(order);
            await _unitOfWork.SaveChangeAsync();

            decimal totalAmount = 0;
            var orderDetails = new List<OrderDetail>();
            var orderDetailsDTO = new List<OrderDetailss>();
            var warningMessages = new List<string>();

            foreach (var item in cartItems)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null) continue;

                int neededQty = item.Quantity;
                decimal totalDetailPrice = 0;
                var batchBreakdown = new List<BatchUsedDTO>();

                var productBatches = batchDetails
                    .Where(b => b.ProductId == item.ProductId && (b.RemainingQuantity ?? 0) > 0)
                    .OrderBy(b => b.ExpiredDate)
                    .ToList();

                foreach (var batch in productBatches)
                {
                    if (neededQty <= 0) break;

                    int available = batch.RemainingQuantity ?? 0;
                    int used = Math.Min(available, neededQty);
                    if (used <= 0) continue;

                    decimal unitPrice = batch.SellingPrice ?? 0;
                    totalDetailPrice += used * unitPrice;

                    //batch.RemainingQuantity -= used;
                    //_unitOfWork.BatchDetailRepo.Update(batch);

                    neededQty -= used;

                    batchBreakdown.Add(new BatchUsedDTO
                    {
                        BatchDetailId = batch.Id,
                        QuantityUsed = used,
                        SellingPrice = unitPrice
                    });
                }

                if (neededQty > 0)
                {
                    warningMessages.Add($"⚠️ Sản phẩm [{product.Name}] chỉ có thể xuất [{item.Quantity - neededQty}/{item.Quantity}].");
                }

                var orderDetail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    OrderId = order.Id,
                    Quantity = item.Quantity,
                    Name = user.Name,
                    Phone = user.Phone,
                    //Address = user.Address,
                    Email = user.Email,
                    UnitPrice = item.Quantity > 0 ? totalDetailPrice / item.Quantity : 0,
                    TotalPrice = totalDetailPrice
                };

                totalAmount += orderDetail.TotalPrice ?? 0;
                orderDetails.Add(orderDetail);

                orderDetailsDTO.Add(new OrderDetailss
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = orderDetail.UnitPrice,
                    TotalPrice = totalDetailPrice,
                    BatchBreakdown = batchBreakdown
                });
            }

            await _unitOfWork.OrderDetailRepo.AddRangeAsync(orderDetails);
            order.OrderAmount = totalAmount;
            _unitOfWork.OrderRepo.Update(order);

            // ✅ XÓA GIỎ HÀNG CỦA USER
             _unitOfWork.CartRepo.SoftRemoveRange(cartItems);

            await _unitOfWork.SaveChangeAsync();

            return new OrderNoLogDTO
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                OrderAmount = order.OrderAmount,
                Name = user.Name,
                Phone = user.Phone,
                //Address = user.Address,
                Message = warningMessages.Any()
                    ? string.Join(" | ", warningMessages)
                    : "Đơn hàng đã được tạo thành công.",
                OrderDetails = orderDetailsDTO
            };
        }

    }
}
