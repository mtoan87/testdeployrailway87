using Application.Commons;
using Application.DTO.Addresses;
using Application.DTO.IntroImages;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories.IntroImages
{
    public interface IIntroImageRepo : IGenericRepository<IntroImage>
    {
        Task<Pagination<IntroImage>> GetPaginationAsync(IntroImagePagination paginationDTO);
    }
}
