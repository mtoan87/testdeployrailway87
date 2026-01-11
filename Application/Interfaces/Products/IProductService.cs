using Application.Commons;
using Application.DTO.Products;
using Application.DTO.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Products
{
    public interface IProductService
    {
        Task<ProductUserListDTO> GetUserProductByIdWithFiltersAsync(int? productId, ProductPaginationDTO paginationDTO);
        Task<ProductListDTO> GetProductByIdAsync(int id);
        Task<ProductUserListDTO> GetProductUserByIdAsync(int id);
        Task<Pagination<ProductUserListDTO>> GetUserProductPaginationAsync(ProductPaginationDTO paginationDTO);
        Task<Pagination<ProductListDTO>> GetPaginationAsync(ProductPaginationDTO paginationDTO);
        Task<IEnumerable<ProductListDTO>> GetProductAsync();
        Task<ProductListDTO> BreakBox(CreateChildProductDTO createProductDto);
        Task DeleteOrEnable(int productId, bool isDeleted);
        Task<ProductListDTO> CreateProduct(CreateProductDTO createProductDto);
        Task<ProductListDTO> UpdateProductAsync(int id, UpdateProductDTO accountDTO);
        Task<bool> UpdateStock(int productId, int quantity, string transactionType, UserInforDTO userInfor);
        //Task<ProductDTO> UpdateProductQuantityAsync(int id, UpdateProductQuantity updateProductDTO);
    }
}
