using Application.Commons;
using Application.DTO.Categories;
using Application.DTO.SourceOfProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.SourceOfProducts
{
    public interface ISourceService
    {
        Task<SourceDTO> AddSource(AddSourceDTO addCategoryDto);
        Task<SourceDTO?> GetSourceById(int id);
        Task<SourceDTO> UpdateSource(int id, UpdateSourceDTO updateCategoryDto);
        Task<List<SourceDTO>> GetAllSource();
        Task<Pagination<SourceOfProductDTO>> GetPaginationAsync(CategoryPaginationDTO paginationDTO);
        Task DeleteOrEnable(int categoryId, bool isDeleted);
    }
}
