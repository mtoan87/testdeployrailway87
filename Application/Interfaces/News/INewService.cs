using Application.Commons;
using Application.DTO.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.News
{
    public interface INewService
    {
        Task DeleteOrEnable(int newId, bool isDeleted);
        Task<List<NewDTO>> GetAllNews();
        Task<NewDTO> UpdateNew(int id, UpdateNewDTO updateNewDTO);
        Task<Pagination<NewDTO>> GetPaginationAsync(NewPaginationDTO paginationDTO);
        Task<NewDTO?> GetNewById(int id);
        Task<NewDTO> AddNew(CreateNewDTO createNewDTO);
    }
}
