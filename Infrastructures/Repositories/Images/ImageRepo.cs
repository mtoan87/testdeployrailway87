using Application.Interfaces;
using Application.IRepositories.Images;
using Application.IRepositories.Orders;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories.Images
{
    public class ImageRepo : GenericRepository<Image>, IImageRepo
    {
        private readonly HypeCatDbContext _context;
        public ImageRepo(
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
