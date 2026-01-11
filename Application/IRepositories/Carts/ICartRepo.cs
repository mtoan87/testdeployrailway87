using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories.Carts
{
    public interface ICartRepo : IGenericRepository<Cart>
    {

        Task<Cart?> GetCartItemByUserAndProduct(int productId);
        Task<List<Cart>> GetUserCartByUserId();
    }
}
