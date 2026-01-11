using Application.Commons;
using Application.DTO.Categories;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Categories
{
    public interface ICategoryService
    {
        Task<Pagination<CategoryDTO>> GetPaginationAsync(CategoryPaginationDTO paginationDTO);
        Task<CategoryDTO> AddCategory(AddCategoryDTO addCategoryDto);
        Task<CategoryDTO?> GetCategoryById(int id);
        Task<CategoryDTO> UpdateCategory(int id, UpdateCategoryDTO updateCategoryDto);
        Task<List<CategoryDTO>> GetAllCategory();
        Task DeleteOrEnable(int categoryId, bool isDeleted);
    }
}
