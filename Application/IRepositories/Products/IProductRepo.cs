using Application.Commons;
using Application.DTO.Dashboards;
using Application.DTO.Products;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories.Products
{
    public interface IProductRepo : IGenericRepository<Product>
    {
        Task<List<Product>> GetByIdsAsync(List<int> ids);
        Task<ProductUserListDTO> GetUserProductByIdWithFiltersAsync(int? productId, ProductPaginationDTO paginationDTO);
        Task<Pagination<ProductUserListDTO>> GetUserProductPaginationAsync(ProductPaginationDTO paginationDTO);
        Task<List<TopProductDTO>> GetTop5BestSellingProductsAsync();
        Task<bool> CheckStockQuantityAsync(int? productId, int requiredQuantity);
        Task<Pagination<Product>> GetPaginationAsync(ProductPaginationDTO paginationDTO);
        Task<Product?> GetProductByBoxIdAsync(int boxId);
        Task<byte[]> ExportInventoryReportAsync(InventoryExportRequest request);
    }
}
