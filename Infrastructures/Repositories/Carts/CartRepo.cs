using Application.Interfaces;
using Application.IRepositories.Carts;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories.Carts
{
    public class CartRepo : GenericRepository<Cart>, ICartRepo
    {
        private readonly HypeCatDbContext _context;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public CartRepo(
            HypeCatDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
            ) :
            base(context, timeService, claimsService)
        {
            _timeService = timeService;
            _context = context;
            _claimsService = claimsService;
        }
        public async Task<Cart?> GetCartItemByUserAndProduct( int productId)
        {
            return await _context.Carts
                .Include(c => c.Product) 
                    .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c =>
                    c.UserId == _claimsService.GetCurrentUserId &&
                    c.ProductId == productId &&
                    (c.IsDeleted == false));
        }



        public async Task<List<Cart>> GetUserCartByUserId()
        {
            var currentUserId = _claimsService.GetCurrentUserId;
            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .ThenInclude(p => p.Images)
                .Where(c => c.UserId == currentUserId && (c.IsDeleted == null || c.IsDeleted == false))
                .ToListAsync();

            return cartItems;
        }


    }
}
