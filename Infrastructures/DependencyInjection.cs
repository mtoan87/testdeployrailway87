using Application.Interfaces;
using Application.Interfaces.Authenticates;
using Application.Interfaces.Users;
using Application.IRepositories.Users;
using Application.Service.Users;
using Application.Service.Authenticates;
using Infrastructures.Mappers;
using Infrastructures.Repositories.Users;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Service;
using Application.IRepositories.Products;
using Infrastructures.Repositories.Products;
using Application.Interfaces.Products;
using Application.Service.Products;
using Application.IRepositories.Records;
using Infrastructures.Repositories.Records;
using Application.IRepositories.Orders;
using Infrastructures.Repositories.Oders;
using Application.Interfaces.Oders;
using Application.Service.Orders;
using Application.IRepositories.OrderDetails;
using Infrastructures.Repositories.OrderDetails;
using Application.IRepositories.Images;
using Domain.Model;
using Infrastructures.Repositories.Images;
using Application.Interfaces.Records;
using Application.Service.Records;
using Application.Interfaces.Dashboards;
using Application.Service.Dashboards;

//using Application.Interfaces.SourceOfProducts;
//using Application.Service.SourceOfProducts;
using Application.IRepositories.Categories;
using Infrastructures.Repositories.Categories;
using Application.Service.Categories;
using Application.Interfaces.Categories;
using Application.Interfaces.Batches;
using Application.Service.BatchDetails;
using Application.Service.Batches;
using Application.IRepositories.Batches;
using Infrastructures.Repositories.Batches;
using Application.IRepositories.BatchDetails;
using Infrastructures.Repositories.BatchDetails;
using Application.Interfaces.BatchDetails;
using Application.IRepositories.Addresses;
using Infrastructures.Repositories.Addresses;
using Application.Interfaces.Addresses;
using Application.Service.Addresses;
using Application.IRepositories.News;
using Infrastructures.Repositories.News;
using Application.Interfaces.News;
using Application.Service.News;
using Application.IRepositories.IntroImages;
using Application.Interfaces.IntroImages;
using Infrastructures.Repositories.IntroImages;
using Application.Service.IntroImages;
using Application.IRepositories.Carts;
using Infrastructures.Repositories.Carts;
using Application.Interfaces.Carts;
using Application.Service.Carts;
using Application.IRepositories.Orderstatuses;

using Infrastructures.Repositories.Orderstatusses;

namespace Infrastructures
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructuresService(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IOrderStatusRepo, OrderStatusRepo>();

            services.AddScoped<IImageRepo, ImageRepo>();
            services.AddScoped<IDashboardService, DashboardService>();

            services.AddScoped<INewRepo, NewRepo>();
            services.AddScoped<INewService, NewService>();

            services.AddScoped<ICartRepo, CartRepo>();
            services.AddScoped<ICartService, CartService>();

            services.AddScoped<IIntroImageRepo, IntroImageRepo>();
            services.AddScoped<IIntroImageService, IntroImageService>();

            //services.AddHostedService<ExpiredBatchLoggerService>();

            services.AddSingleton<ICloudinaryService, CloudinaryService>();

            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IProductRepo, ProductRepo>();
            services.AddScoped<IProductService, ProductService>();

            services.AddScoped<IOrderRepo, OrderRepo>();
            services.AddScoped<IOrderService, OrderService>();

            services.AddScoped<IAddressRepo, AddressRepo>();
            services.AddScoped<IAddressService, AddressService>();

            //services.AddScoped<ISourceRepo, SourceRepo>();
            //services.AddScoped<ISourceService, SourceService>();

            services.AddScoped<ICateRepo, CateRepo>();
            services.AddScoped<ICategoryService, CategoryService>();

            services.AddScoped<IBatchService, BatchService>();
            services.AddScoped<IBatchRepo, BatchRepo>();

            services.AddScoped<IBatchDetailRepo, BatchDetailRepo>();
            services.AddScoped<IBatchDetailService, BatchDetailService>();

            services.AddScoped<IOrderDetailRepo, OrderDetailRepo>();

            services.AddScoped<ILogRepo, LogRepo>();
            services.AddScoped<IRecordService, RecordService>();

            services.AddScoped<IAuthenticatesService, AuthenticateService>();

            services.AddSingleton<ICurrentTime, CurrentTime>();

            // ATTENTION: if you do migration please check file README.md
            services.AddDbContext<HypeCatDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionString")));

            // this configuration just use in-memory for fast develop
            //services.AddDbContext<AppDbContext>(option => option.UseInMemoryDatabase("test"));

            services.AddMapsterConfigurations();

            return services;
        }
    }
}
