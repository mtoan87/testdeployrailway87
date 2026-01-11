using Application.DTO.Categories;
using Application.DTO.Products;
using Application.Interfaces.Categories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NgocBichKiot.Api.Services.Examples;
using Swashbuckle.AspNetCore.Filters;

namespace NgocBichKiot.Api.Controllers.Categories
{
    public class CategoryController : BaseController
    {
        private readonly ICategoryService service;
        public CategoryController(ICategoryService categoryService )
        {
            service = categoryService;
        }

        [HttpGet]
       
        public async Task<IActionResult> GetCategoryPagination([FromQuery] CategoryPaginationDTO paginationDTO)
        {
            try
            {
                var result = await service.GetPaginationAsync(paginationDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] AddCategoryDTO addCategoryDto)
        {
            try
            {
                var result = await service.AddCategory(addCategoryDto);
                return Created(nameof(CreateCategory), result);
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDTO updateCategoryDto)
        {
           
            try
            {
                var result = await service.UpdateCategory(id, updateCategoryDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPut("{categoryId}/{isDeleted}")]
        public async Task<IActionResult> DeleteOrEnable(int categoryId, int isDeleted)
        {
          

            try
            {
                await service.DeleteOrEnable(categoryId, isDeleted > 0);
                return NoContent();
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, new { message = ex.Message });
            }

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var result = await service.GetCategoryById(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await service.GetAllCategory();
            return Ok(result);
        }
    }
}
