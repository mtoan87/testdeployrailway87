using Application.Commons;
using Application.DTO.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.IRepositories.News
{
    public interface INewRepo : IGenericRepository<Domain.Model.News>
    {
        Task<Pagination<Domain.Model.News>> GetPaginationAsync(NewPaginationDTO paginationDTO);
    }
}
