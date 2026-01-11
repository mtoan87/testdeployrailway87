using Application.DTO.Categories;
using Application.Interfaces;
using Application.Interfaces.Categories;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using Application.Commons;
using Application.DTO.Products;
using CloudinaryDotNet;

namespace Application.Service.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Pagination<CategoryDTO>> GetPaginationAsync(CategoryPaginationDTO paginationDTO)
        {
            try
            {
                var pagination = await _unitOfWork.CateRepo.GetPaginationAsync(paginationDTO);

                return new Pagination<CategoryDTO>
                {
                    Items = pagination.Items.Adapt<List<CategoryDTO>>(),
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
        public async Task<CategoryDTO> AddCategory(AddCategoryDTO addCategoryDto)
        {
            // Kiểm tra trùng tên
            var existingCategory = await _unitOfWork.CateRepo.GetAsync(c => c.Name == addCategoryDto.Name && !c.IsDeleted);
            if (existingCategory != null)
            {
                throw new Exception("Category already exists.");
            }

            var category = addCategoryDto.Adapt<Category>();
            await _unitOfWork.CateRepo.AddAsync(category);
            await _unitOfWork.SaveChangeAsync();
            if (addCategoryDto.CateImages != null && addCategoryDto.CateImages.Any())
            {
                foreach (var url in addCategoryDto.CateImages)
                {
                    var image = new Image
                    {
                        UrlPath = url,
                        CategoryId = category.Id,

                    };
                    await _unitOfWork.ImageRepo.AddAsync(image);
                }
            }

            await _unitOfWork.SaveChangeAsync();
            return category.Adapt<CategoryDTO>();
        }

        public async Task<CategoryDTO?> GetCategoryById(int id)
        {
            var category = await _unitOfWork.CateRepo.GetById(id, includeProperties:"Images");
            if (category is null)
            {
                throw new Exception("Category is not existed");
            }

            return category.Adapt<CategoryDTO>();
        }

        public async Task<CategoryDTO> UpdateCategory(int id, UpdateCategoryDTO updateCategoryDto)
        {
            var category = await _unitOfWork.CateRepo.GetByIdAsync(id);
            if (category is null)
            {
                throw new Exception("Category does not exist.");
            }

            // Kiểm tra xem có sản phẩm nào đang dùng category này chưa
            var isUsedInProduct = await _unitOfWork.ProductRepo.AnyAsync(p => p.CategoryId == id);
            if (isUsedInProduct)
            {
                throw new Exception("Cannot update category because it is in use by some products.");
            }

            updateCategoryDto.Adapt(category); // Gán các giá trị mới vào category
            _unitOfWork.CateRepo.Update(category);
            await _unitOfWork.SaveChangeAsync();
            if (updateCategoryDto.CateImages != null)
            {
                var existingImages = await _unitOfWork.ImageRepo.GetAllAsync(x => x.CategoryId == id);
                var existingUrls = existingImages.Select(i => i.UrlPath).ToList();
                var newUrls = updateCategoryDto.CateImages;

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
                        CategoryId = id,

                    };
                    await _unitOfWork.ImageRepo.AddAsync(image);
                }

                await _unitOfWork.SaveChangeAsync();
            }
            return category.Adapt<CategoryDTO>();
        }


        public async Task<List<CategoryDTO>> GetAllCategory()
        {
            var categories = await _unitOfWork.CateRepo.GetAllAsync();
            return categories.Adapt<List<CategoryDTO>>();
        }

        public async Task DeleteOrEnable(int categoryId, bool isDeleted)
        {
            var category = await _unitOfWork.CateRepo.GetAsync(d => d.Id == categoryId);
            if (category is null)
            {
                throw new Exception("Category does not exist.");
            }

            // Nếu đang cố gắng xóa (isDeleted = true) thì kiểm tra xem có sản phẩm nào đang dùng không
            if (isDeleted)
            {
                var isUsedInProduct = await _unitOfWork.ProductRepo.AnyAsync(p => p.CategoryId == categoryId);
                if (isUsedInProduct)
                {
                    throw new Exception("Cannot delete category because it is in use by some products.");
                }
            }

            category.IsDeleted = isDeleted;
            await _unitOfWork.SaveChangeAsync();
        }
    }
}
