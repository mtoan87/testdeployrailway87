using Application.DTO.Categories;
using Application.DTO.Orders;
using Application.DTO.Products;
using Application.DTO.Users;
using Application.Interfaces.Products;
using Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NgocBichKiot.Api.Services.Examples;
using Swashbuckle.AspNetCore.Filters;

namespace NgocBichKiot.Api.Controllers.Products
{   
    public class ProductsController : BaseController
    {
        private readonly IProductService _productService;
       

        public ProductsController(IProductService productService)
        {
            _productService = productService;
            
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<ProductListDTO>), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ProductListDTOExample))]
        public async Task<IActionResult> GetProductList()
        {            
            try
            {
                var User = await _productService.GetProductAsync();
                return Ok(User);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
       
        [HttpGet]
        [ProducesResponseType(typeof(List<ProductListDTO>), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ProductListDTOExample))]
        public async Task<IActionResult> GetProductPagination([FromQuery] ProductPaginationDTO paginationDTO)
        {            
            try
            {
                var result = await _productService.GetPaginationAsync(paginationDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet]
        //[ProducesResponseType(typeof(List<ProductListDTO>), StatusCodes.Status200OK)]
        //[SwaggerResponseExample(StatusCodes.Status200OK, typeof(ProductListDTOExample))]
        public async Task<IActionResult> GetUserProductPagination([FromQuery] ProductPaginationDTO paginationDTO)
        {
            try
            {
                var result = await _productService.GetUserProductPaginationAsync(paginationDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(List<ProductListDTO>), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ProductListDTOExample))]
        public async Task<IActionResult> GetProductById(int id)
        {
            
            try
            {
                var findAccountUser = await _productService.GetProductByIdAsync(id);
                return Ok(findAccountUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet("{id}")]
        
        public async Task<IActionResult> GetProductUserById(int id)
        {

            try
            {
                var findAccountUser = await _productService.GetProductUserByIdAsync(id);
                return Ok(findAccountUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetProductByIdWithFilters(int? id, [FromQuery] ProductPaginationDTO filters)
        //{
        //    try
        //    {
        //        var product = await _productService.GetUserProductByIdWithFiltersAsync(id ?? 0, filters);
        //        return Ok(product);
        //    }
        //    catch (Exception ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
        //}
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO createProduct)
        {
           
            try
            {
                var response = await _productService.CreateProduct(createProduct);
                return Created(nameof(CreateProduct), response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BreakBox([FromBody] CreateChildProductDTO createProduct)
        {
           
            try
            {
                var response = await _productService.BreakBox(createProduct);
                return Created(nameof(BreakBox), response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDTO accountDTO)
        {           
            try
            {
                var updatedProd = await _productService.UpdateProductAsync(id, accountDTO);
                return Ok(updatedProd);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateProductQuantity(int id, [FromBody] UpdateProductQuantity accountDTO)
        //{
        //    var updatedProd = await _productService.UpdateProductQuantityAsync(id, accountDTO);
        //    return Ok(updatedProd);
        //}
        [HttpPut()]
        public async Task<IActionResult> UpdateStock(int productId, int quantity, string type, UserInforDTO userInfor)
        {
            
            try
            {
                var updatedProd = await _productService.UpdateStock(productId, quantity, type, userInfor);
                return Ok(updatedProd);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut()]
        public async Task<IActionResult> DeleteOrEnable(int productId, int isDeleted)
        {
            
            try
            {
                await _productService.DeleteOrEnable(productId, isDeleted > 0);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }
    }
}
