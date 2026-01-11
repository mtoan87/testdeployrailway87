using Application.DTO.BatchDetails;
using Application.DTO.Batches;
using Application.DTO.Categories;
using Application.DTO.Images;
using Application.DTO.News;
using Application.DTO.Orders;
using Application.DTO.Products;
using Application.DTO.Records;
using Application.DTO.SourceOfProducts;
using Application.DTO.Users;
using Domain.Model;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Mappers
{
    public static class MapsterConfig
    {
        public static IServiceCollection AddMapsterConfigurations(this IServiceCollection services)
        {
            services.AddMapster();
            TypeAdapterConfig<Image, ImageDTO>.NewConfig();
            TypeAdapterConfig<UpdateProductDTO, Product>.NewConfig().IgnoreNullValues(true);
            TypeAdapterConfig<UpdateNewDTO, Domain.Model.News>.NewConfig().IgnoreNullValues(true);
            TypeAdapterConfig<UpdateUserDTO, User>.NewConfig().IgnoreNullValues(true);
            TypeAdapterConfig<UpdateOrderDTO, Order>.NewConfig().IgnoreNullValues(true);
            TypeAdapterConfig<UpdateLogDTO, Log>.NewConfig().IgnoreNullValues(true);
            TypeAdapterConfig<UpdateCategoryDTO, Category>.NewConfig().IgnoreNullValues(true);
            //TypeAdapterConfig<UpdateSourceDTO, SourceOfProduct>.NewConfig().IgnoreNullValues(true);
            TypeAdapterConfig<UpdateBatchDTO, Batch>.NewConfig().IgnoreNullValues(true);
            TypeAdapterConfig<UpdateBatchDetailDTO, BatchDetail>.NewConfig().IgnoreNullValues(true);
            TypeAdapterConfig<BatchDetail, BatchDetailDTO>.NewConfig()
            //.Map(dest => dest.BatchDTO, src => src.Batch)
            .Map(dest => dest.ProductDTO, src => src.Product);
            //.Map(dest => dest.SourceOfProductDTO, src => src.SourceOfProduct);
            TypeAdapterConfig<BatchDetail, BatchDetailProductDTO>.NewConfig();
            //.Map(dest => dest.BatchDTO, src => src.Batch)
            //.Map(dest => dest.ProductDTO, src => src.Product)
            //.Map(dest => dest.SourceOfProductDTO, src => src.SourceOfProduct);
            TypeAdapterConfig<BatchDetail, BatchDetailNProductDTO>.NewConfig();
           //.Map(dest => dest.BatchDTO, src => src.Batch)
           //.Map(dest => dest.ProductDTO, src => src.Product)
           //.Map(dest => dest.SourceOfProductDTO, src => src.SourceOfProduct);
            TypeAdapterConfig<Batch, BatchWDetailDTO>.NewConfig()
            .Map(dest => dest.BatchDetailDTOs, src => src.BatchDetails);
            return services;
        }
    }
}
