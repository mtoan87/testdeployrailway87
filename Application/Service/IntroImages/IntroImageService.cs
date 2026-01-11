using Application.Commons;
using Application.DTO.Addresses;
using Application.DTO.IntroImages;
using Application.DTO.News;
using Application.Interfaces;
using Application.Interfaces.IntroImages;
using Domain.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.IntroImages
{
    public class IntroImageService : IIntroImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        public IntroImageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Pagination<IntroImageDTO>> GetPaginationAsync(IntroImagePagination paginationDTO)
        {
            try
            {
                var pagination = await _unitOfWork.IntroImageRepo.GetPaginationAsync(paginationDTO);

                return new Pagination<IntroImageDTO>
                {
                    Items = pagination.Items.Adapt<List<IntroImageDTO>>(),
                    TotalItemsCount = pagination.TotalItemsCount,
                    PageSize = pagination.PageSize,
                    PageIndex = pagination.PageIndex
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting pagination: {ex.Message}", ex);
            }
        }
        public async Task<IntroImageDTO> AddIntroImage(CreateIntroImage create)
        {
            var category = create.Adapt<IntroImage>();
            await _unitOfWork.IntroImageRepo.AddAsync(category);
            await _unitOfWork.SaveChangeAsync();
            return category.Adapt<IntroImageDTO>();
        }

        public async Task DeleteOrEnable(int imageId, bool isDeleted)
        {
            var category = await _unitOfWork.IntroImageRepo.GetAsync(d => d.Id == imageId);
            if (category is null)
            {
                throw new Exception("Img does not exist.");
            }
            category.IsDeleted = isDeleted;
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task<List<IntroImageDTO>> GetAllIntroImages()
        {
            var categories = await _unitOfWork.IntroImageRepo.GetAllAsync();
            return categories.Adapt<List<IntroImageDTO>>();
        }

        public async Task<IntroImageDTO?> GetIntroImageById(int id)
        {
            var category = await _unitOfWork.IntroImageRepo.GetAsync(x => x.Id == id);
            if (category is null)
            {
                throw new Exception("Img is not existed");
            }

            return category.Adapt<IntroImageDTO>();
        }

        public async Task<IntroImageDTO> UpdateIntroImage(int id, UpdateIntroImage update)
        {
            var news = await _unitOfWork.IntroImageRepo.GetByIdAsync(id);
            if (news is null)
            {
                throw new Exception("News does not exist.");
            }



            update.Adapt(news);
            _unitOfWork.IntroImageRepo.Update(news);
            await _unitOfWork.SaveChangeAsync();
            return news.Adapt<IntroImageDTO>();
        }
    }
}
