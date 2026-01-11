using Application.Commons;
using Application.DTO.Categories;
using Application.DTO.News;
using Application.DTO.Products;
using Application.DTO.Users;
using Application.Interfaces;
using Application.Interfaces.News;
using CloudinaryDotNet;
using Domain.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.News
{
    public class NewService : INewService
    {
        private readonly IUnitOfWork _unitOfWork;
        public NewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Pagination<NewDTO>> GetPaginationAsync(NewPaginationDTO paginationDTO)
        {
            try
            {
                var pagination = await _unitOfWork.NewRepo.GetPaginationAsync(paginationDTO);

                return new Pagination<NewDTO>
                {
                    Items = pagination.Items.Adapt<List<NewDTO>>(),
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
        public async Task<NewDTO> AddNew(CreateNewDTO createNewDTO)
        {
            try
            {
                var category = await _unitOfWork.CateRepo.GetByIdAsync(createNewDTO.CategoryId);
                if (category == null || category.CateType != "News")
                {
                    throw new Exception("Loại tin tức không hợp lệ!");
                }

                var news = createNewDTO.Adapt<Domain.Model.News>();

                await _unitOfWork.NewRepo.AddAsync(news);
                await _unitOfWork.SaveChangeAsync();

                if (createNewDTO.NewsImages != null && createNewDTO.NewsImages.Any())
                {
                    foreach (var url in createNewDTO.NewsImages)
                    {
                        var image = new Image
                        {
                            UrlPath = url,
                            NewsId = news.Id,
                        };
                        await _unitOfWork.ImageRepo.AddAsync(image);
                    }
                }
                await _unitOfWork.SaveChangeAsync();

                news = await _unitOfWork.NewRepo.GetById(news.Id, includeProperties: "Images,Category");
                return news.Adapt<NewDTO>();
            }
            catch (Exception ex)
            {
                // Ghi log ra console hoặc dùng ILogger nếu bạn có inject
                Console.WriteLine($"[AddNew] Exception: {ex.Message} \n {ex.StackTrace}");
                throw; // Rethrow để middleware xử lý hoặc trả về lỗi đúng chuẩn API
            }
        }

        public async Task<NewDTO?> GetNewById(int id)
        {
            var category = await _unitOfWork.NewRepo.GetAsync(x => x.Id == id , includeProperties:"Images,Category");
            if (category is null)
            {
                throw new Exception("News is not existed");
            }
            category.Images = category.Images
       .Where(img => !img.IsDeleted)
       .ToList();


            return category.Adapt<NewDTO>();
        }

        public async Task<NewDTO> UpdateNew(int id, UpdateNewDTO updateNewDTO)
        {
            var news = await _unitOfWork.NewRepo.GetByIdAsync(id);
            if (news is null)
            {
                throw new Exception("News does not exist.");
            }

            // Cập nhật các thuộc tính cơ bản
            updateNewDTO.Adapt(news);
            news.ModifiedDate = DateTime.Now;

            _unitOfWork.NewRepo.Update(news);
            await _unitOfWork.SaveChangeAsync();

            if (updateNewDTO.NewsImages != null)
            {
                var existingImages = await _unitOfWork.ImageRepo.GetAllAsync(x => x.NewsId == id);
                var existingUrls = existingImages.Select(i => i.UrlPath).ToList();
                var newUrls = updateNewDTO.NewsImages;

                // 1. Xóa ảnh cũ không còn sử dụng (xóa cứng)
                var toDelete = existingImages.Where(i => !newUrls.Contains(i.UrlPath)).ToList();
                foreach (var img in toDelete)
                {
                    await _unitOfWork.ImageRepo.DeleteAsync(img);
                }

                // 2. Thêm ảnh mới
                var toAdd = newUrls.Where(url => !existingUrls.Contains(url)).ToList();
                foreach (var url in toAdd)
                {
                    var image = new Image
                    {
                        UrlPath = url,
                        NewsId = id,

                    };
                    await _unitOfWork.ImageRepo.AddAsync(image);
                }

                await _unitOfWork.SaveChangeAsync();
            }

            // Trả về DTO bao gồm ảnh
            return news.Adapt<NewDTO>();


           
        }


        public async Task<List<NewDTO>> GetAllNews()
        {
            var categories = await _unitOfWork.NewRepo.GetAllAsync(
                filter: n => !n.IsDeleted,
                includeProperties: "Images,Category"
            );
            return categories.Adapt<List<NewDTO>>();
        }

        public async Task DeleteOrEnable(int newId, bool isDeleted)
        {
            var category = await _unitOfWork.NewRepo.GetAsync(d => d.Id == newId);
            if (category is null)
            {
                throw new Exception("News does not exist.");
            }
            category.IsDeleted = isDeleted;
            await _unitOfWork.SaveChangeAsync();
        }
    }
}
