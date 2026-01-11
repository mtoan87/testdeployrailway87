using Application.IRepositories.Records;
using Application.IRepositories.OrderDetails;
using Application.IRepositories.Orders;
using Application.IRepositories.Products;
using Application.IRepositories.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IRepositories.Images;
using Application.IRepositories.Categories;
using Application.IRepositories.Batches;
using Application.IRepositories.BatchDetails;
using Application.IRepositories.Addresses;
using Application.IRepositories.News;
using Application.IRepositories.IntroImages;
using Application.IRepositories.Carts;
using Application.IRepositories.Orderstatuses;

namespace Application.Interfaces
{
    public interface IUnitOfWork
    {
        public IOrderStatusRepo OrderStatusRepo { get; }
        public ICartRepo CartRepo { get; }
        public IIntroImageRepo IntroImageRepo { get; }
        public INewRepo NewRepo { get; }
        public IAddressRepo AddressRepo { get; }
        public ICateRepo CateRepo { get; }      
        public IImageRepo ImageRepo { get; }
        public IOrderDetailRepo OrderDetailRepo { get; }
        public IOrderRepo OrderRepo { get; }
        public IUserRepo UserRepo {  get; }
        public IProductRepo ProductRepo {  get; }
        public ILogRepo LogRepo { get; }
            
        public IBatchRepo BatchRepo { get; }

        public IBatchDetailRepo BatchDetailRepo { get; }
        public Task<int> SaveChangeAsync();
    }
}
