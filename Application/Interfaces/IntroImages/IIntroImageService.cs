using Application.Commons;
using Application.DTO.IntroImages;
using Application.DTO.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IntroImages
{
    public interface IIntroImageService
    {
        Task DeleteOrEnable(int imageId, bool isDeleted);
        Task<List<IntroImageDTO>> GetAllIntroImages();
        Task<IntroImageDTO> UpdateIntroImage(int id, UpdateIntroImage update);
        Task<Pagination<IntroImageDTO>> GetPaginationAsync(IntroImagePagination paginationDTO);
        
        Task<IntroImageDTO?> GetIntroImageById(int id);
        Task<IntroImageDTO> AddIntroImage(CreateIntroImage create);
    }
}
