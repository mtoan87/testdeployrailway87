using Application.Commons;
using Application.DTO.Categories;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories.Categories
{
    public interface ICateRepo : IGenericRepository<Category>
    {
        Task<Pagination<Category>> GetPaginationAsync(CategoryPaginationDTO paginationDTO);
    }
}
