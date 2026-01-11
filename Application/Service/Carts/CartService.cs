using Application.DTO.Addresses;
using Application.DTO.Carts;
using Application.DTO.Users;
using Application.Interfaces;
using Application.Interfaces.Carts;
using Application.IRepositories.BatchDetails;
using Application.IRepositories.Carts;
using Domain.Model;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Carts
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;

        public CartService(IUnitOfWork unitOfWork,
            IClaimsService claimsService)
        {
            _unitOfWork = unitOfWork;
            _claimsService = claimsService;
        }

        


        public async Task<CartDTO?> GetCartById(int id)
        {
            var category = await _unitOfWork.CartRepo.GetAsync(x => x.Id == id);
            if (category is null)
            {
                throw new Exception("Cart is not existed");
            }

            return category.Adapt<CartDTO>();
        }

        public async Task<CartDTO> UpdateCart(int id, UpdateCartDTO updateCart)
        {
            var address = await _unitOfWork.CartRepo.GetByIdAsync(id);
            if (address is null)
            {
                throw new Exception("Address does not exist.");
            }
            updateCart.Adapt(address);
            _unitOfWork.CartRepo.Update(address);
            await _unitOfWork.SaveChangeAsync();
            return address.Adapt<CartDTO>();
        }


        public async Task<List<CartDTO>> GetCarts()
        {
            var categories = await _unitOfWork.CartRepo.GetAllAsync();
            return categories.Adapt<List<CartDTO>>();
        }

        public async Task<List<CartItemDisplayDTO>> GetUserCartDisplayAsync()
        {
            var carts = await _unitOfWork.CartRepo.GetUserCartByUserId();

            if (carts == null || !carts.Any())
                throw new Exception("Giỏ hàng trống.");

            var result = new List<CartItemDisplayDTO>();

            foreach (var item in carts)
            {
                var batchDetails = await _unitOfWork.BatchDetailRepo.GetFirstValidBatchDetails(item.ProductId);
                var totalRemaining = batchDetails.Sum(b => b.RemainingQuantity ?? 0);

                if (totalRemaining < item.Quantity)
                    throw new Exception($"Sản phẩm {item.Product?.Name ?? item.ProductId.ToString()} không đủ tồn kho.");

                // Chỉ cần 1 BatchDetail để lấy thông tin giá và ID
                var firstDetail = batchDetails.OrderBy(b => b.CreateDate).First();

                var firstImage = item.Product?.Images?
                    .FirstOrDefault(img => img.IsDeleted == false)?.UrlPath ?? "";

                result.Add(new CartItemDisplayDTO
                {
                    CartId = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name ?? "Unknown",
                    ImageUrl = firstImage,
                    Quantity = item.Quantity,
                    BatchDetailId = firstDetail.Id,
                    RemainingQuantity = totalRemaining,
                    SellingPrice = firstDetail.SellingPrice ?? 0,
                    TotalPrice = (firstDetail.SellingPrice ?? 0) * item.Quantity
                });
            }

            return result;
        }

        public async Task DeleteOrEnable(int cartId, bool isDeleted)
        {
            var address = await _unitOfWork.CartRepo.GetAsync(d => d.Id == cartId);
            if (address is null)
            {
                throw new Exception("Cart does not exist.");
            }
            address.IsDeleted = isDeleted;
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task<CartDTO> UpdateCartItemAsync(UpdateCartItemDTO updateCart)
        {
            var cartDetail = await _unitOfWork.CartRepo.GetAsync(cd =>
                cd.Id == updateCart.CartId, includeProperties: "Product.BatchDetails,Product.Images");

            if (cartDetail == null)
            {
                throw new Exception("Cart item not found.");
            }

            // Update quantity
            cartDetail.Quantity = updateCart.Quantity;

            _unitOfWork.CartRepo.Update(cartDetail);
            await _unitOfWork.SaveChangeAsync();

            return cartDetail.Adapt<CartDTO>();
        }

        public async Task DeleteCartItemAsync(int productId)
        {
            var currentUserId = _claimsService.GetCurrentUserId;

            var cartItem = await _unitOfWork.CartRepo.GetCartItemByUserAndProduct(
                 productId);

            if (cartItem == null)
            {
                throw new Exception("Không tìm thấy sản phẩm trong giỏ hàng.");
            }

            cartItem.IsDeleted = true; // Hoặc _context.Carts.Remove(cartItem); nếu bạn muốn xóa luôn
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task CreateCart(CreateCartDTO create)
        {
            if (create.Quantity <= 0)
                throw new ArgumentException("Số lượng phải lớn hơn 0.");

            if (create == null)
                throw new Exception("CreateCartDTO is null");

            if (_unitOfWork == null)
                throw new Exception("UnitOfWork is not injected properly");

            if (_unitOfWork.CartRepo == null)
                throw new Exception("CartRepo is null");
            // Tìm dòng sản phẩm đã có trong giỏ hàng của user (chưa bị xóa)
            var existingCartItem = await _unitOfWork.CartRepo.GetCartItemByUserAndProduct(create.ProductId);

            if (existingCartItem != null)
            {
                // Nếu đã có thì cộng thêm
                existingCartItem.Quantity += create.Quantity;
               
                 _unitOfWork.CartRepo.Update(existingCartItem);
                await _unitOfWork.SaveChangeAsync();
            }
            else
            {
                // Nếu chưa có thì tạo mới và lấy giá từ Batch gần nhất
                var batch = await _unitOfWork.BatchDetailRepo.GetBatchDetailsByProductIdAsync(create.ProductId);

                var firstBatch = batch.FirstOrDefault();

                if (firstBatch == null || firstBatch.RemainingQuantity == null)
                    throw new Exception("Không còn hàng trong kho cho sản phẩm này.");

                if (create.Quantity > firstBatch.RemainingQuantity)
                    throw new Exception("Số lượng yêu cầu vượt quá số lượng còn lại trong kho.");

                var newCart = new Cart
                {
                    UserId = _claimsService.GetCurrentUserId,
                    ProductId = create.ProductId,
                    Quantity = create.Quantity,
                    UnitPrice = firstBatch.SellingPrice ?? 0,
                };

                await _unitOfWork.CartRepo.AddAsync(newCart);
            }

            await _unitOfWork.SaveChangeAsync();
        }

        
    }
}
