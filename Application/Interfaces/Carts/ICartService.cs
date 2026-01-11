using Application.DTO.Carts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Carts
{
    public interface ICartService
    {
        Task<CartDTO> UpdateCartItemAsync(UpdateCartItemDTO updateCart);
        Task DeleteCartItemAsync(int productId);
        Task<List<CartItemDisplayDTO>> GetUserCartDisplayAsync();
        Task CreateCart(CreateCartDTO create);
        Task<CartDTO?> GetCartById(int id);
        Task<CartDTO> UpdateCart(int id, UpdateCartDTO updateCart);
        Task<List<CartDTO>> GetCarts();
        //Task<CartDTO> GetUserCartByUserId();
        Task DeleteOrEnable(int cartId, bool isDeleted);
    }
}
