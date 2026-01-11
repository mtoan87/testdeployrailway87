using Application.Interfaces;
using Application.IRepositories.Orderstatuses;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories.Orderstatusses
{
    public class OrderStatusRepo : GenericRepository<OrderStatus>, IOrderStatusRepo
    {
        private readonly HypeCatDbContext _context;
        public OrderStatusRepo(
            HypeCatDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
            ) :
            base(context, timeService, claimsService)
        {
            _context = context;
        }
    }
}
