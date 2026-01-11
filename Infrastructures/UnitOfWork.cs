using Application.Interfaces;
using Application.IRepositories.Records;
using Application.IRepositories.Products;
using Application.IRepositories.Users;
using Application.IRepositories.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IRepositories.OrderDetails;
using Application.IRepositories.Images;
using Infrastructures.Repositories.Images;
using Application.IRepositories.Categories;
//using Application.IRepositories.SourceOfProducts;
using Application.IRepositories.BatchDetails;
using Application.IRepositories.Batches;
using Application.IRepositories.Addresses;
using Application.IRepositories.News;
using Application.IRepositories.IntroImages;
using Application.IRepositories.Carts;
using Application.IRepositories.Orderstatuses;

namespace Infrastructures
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HypeCatDbContext _context;
        private readonly INewRepo _newRepo;
        private readonly IOrderStatusRepo _orderStatusRepo;
        private readonly IIntroImageRepo _introImageRepo;
        private readonly ICartRepo _cartRepo;
        private readonly IUserRepo _userRepo;
        private readonly IProductRepo _productRepo;
        private readonly ILogRepo _logRepo;
        private readonly IOrderRepo _orderRepo;
        private readonly IOrderDetailRepo _orderDetailRepo;
        private readonly IImageRepo _imageRepo;
        private readonly ICateRepo _cateRepo;
        private readonly IAddressRepo _addressRepo;
      
        private readonly IBatchDetailRepo _batchDetailRepo;
        private readonly IBatchRepo _batchRepo;
        public UnitOfWork
        (

            HypeCatDbContext context,
           
            ICartRepo cartRepo,
            IOrderStatusRepo orderStatusRepo,
            IIntroImageRepo introImageRepo,
            INewRepo newRepo,
            IUserRepo userRepo,
            IProductRepo productRepo,
            ILogRepo logRepo,
            IOrderRepo orderRepo,
            IOrderDetailRepo orderDetailRepo,
            IImageRepo imageRepo,
            ICateRepo cateRepo,
           
            IAddressRepo addressRepo,
            IBatchDetailRepo batchDetailRepo,
            IBatchRepo batchRepo
        )
        {
            _addressRepo = addressRepo;
            _cartRepo = cartRepo;
            _orderStatusRepo = orderStatusRepo;
            _introImageRepo = introImageRepo;
            _newRepo = newRepo;         
            _imageRepo = imageRepo;
            _orderDetailRepo = orderDetailRepo;
            _orderRepo = orderRepo;
            _userRepo = userRepo;
            _productRepo = productRepo;
            _logRepo = logRepo;
            _context = context;
            _cateRepo = cateRepo;
          
            _batchDetailRepo = batchDetailRepo;
            _batchRepo = batchRepo;
        }

        public IOrderStatusRepo OrderStatusRepo => _orderStatusRepo;
        public ICartRepo CartRepo => _cartRepo;
        public IIntroImageRepo IntroImageRepo => _introImageRepo;
        public INewRepo NewRepo => _newRepo;
        public IAddressRepo AddressRepo => _addressRepo;
        public IOrderDetailRepo OrderDetailRepo => _orderDetailRepo;
        public IImageRepo ImageRepo => _imageRepo;
        public IOrderRepo OrderRepo => _orderRepo;
        public ILogRepo LogRepo => _logRepo;
        public IProductRepo ProductRepo => _productRepo;
        public IUserRepo UserRepo => _userRepo;
        public IBatchDetailRepo BatchDetailRepo => _batchDetailRepo;
        public IBatchRepo BatchRepo => _batchRepo;
       
        public ICateRepo CateRepo => _cateRepo;
        public async Task<int> SaveChangeAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
