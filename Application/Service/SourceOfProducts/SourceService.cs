//using Application.Commons;
//using Application.DTO.Categories;
//using Application.DTO.SourceOfProducts;
//using Application.Interfaces;
//using Application.Interfaces.SourceOfProducts;
//using Domain.Model;
//using Mapster;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Application.Service.SourceOfProducts
//{
//    public class SourceService : ISourceService
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        public SourceService(IUnitOfWork unitOfWork)
//        {
//            _unitOfWork = unitOfWork;
//        }
//        public async Task<Pagination<SourceOfProductDTO>> GetPaginationAsync(CategoryPaginationDTO paginationDTO)
//        {
//            try
//            {
//                var pagination = await _unitOfWork.SourceRepo.GetPaginationAsync(paginationDTO);

//                return new Pagination<SourceOfProductDTO>
//                {
//                    Items = pagination.Items.Adapt<List<SourceOfProductDTO>>(),
//                    TotalItemsCount = pagination.TotalItemsCount,
//                    PageSize = pagination.PageSize,
//                    PageIndex = pagination.PageIndex
//                };
//            }
//            catch (Exception ex)
//            {
//                throw new Exception($"An error occurred while getting pagination: {ex.Message}", ex);
//            }
//        }
//        public async Task<SourceDTO> AddSource(AddSourceDTO addCategoryDto)
//        {
//            var existingCategory = await _unitOfWork.SourceRepo.GetAsync(c => c.Name == addCategoryDto.Name && !c.IsDeleted);
//            if (existingCategory != null)
//            {
//                throw new Exception("SourceOfProduct already exists.");
//            }
//            var category = addCategoryDto.Adapt<SourceOfProduct>();
//            await _unitOfWork.SourceRepo.AddAsync(category);
//            await _unitOfWork.SaveChangeAsync();
//            return category.Adapt<SourceDTO>();
//        }

//        public async Task DeleteOrEnable(int categoryId, bool isDeleted)
//        {
//            var category = await _unitOfWork.SourceRepo.GetAsync(d => d.Id == categoryId);
//            if (category is null)
//            {
//                throw new Exception("SourceOfProduct is not existed");
//            }
//            //if (isDeleted)
//            //{
//            //    var isUsedInProduct = await _unitOfWork.ProductRepo.AnyAsync(p => p.SourceOfProductId == categoryId);
//            //    if (isUsedInProduct)
//            //    {
//            //        throw new Exception("Cannot delete SourceOfProduct because it is in use by some products.");
//            //    }
//            //}
//            category.IsDeleted = isDeleted;
//            await _unitOfWork.SaveChangeAsync();
//        }

//        public async Task<List<SourceDTO>> GetAllSource()
//        {
//            var categories = await _unitOfWork.SourceRepo.GetAllAsync();
//            return categories.Adapt<List<SourceDTO>>();
//        }

//        public async Task<SourceDTO?> GetSourceById(int id)
//        {
//            var category = await _unitOfWork.SourceRepo.GetAsync(x => x.Id == id);
//            if (category is null)
//            {
//                throw new Exception("SourceOfProduct is not existed");
//            }

//            return category.Adapt<SourceDTO>();
//        }

//        public async Task<SourceDTO> UpdateSource(int id, UpdateSourceDTO updateCategoryDto)
//        {
//            var category = await _unitOfWork.SourceRepo.GetByIdAsync(id);
//            if (category is null)
//            {
//                throw new Exception("SourceOfProduct is not existed");
//            }
//            var isUsedInProduct = await _unitOfWork.BatchDetailRepo.AnyAsync(p => p.SourceOfProductId == id);
//            if (isUsedInProduct)
//            {
//                throw new Exception("Cannot update sourceofproduct because it is in use by some batches.");
//            }
//            _unitOfWork.SourceRepo.Update(updateCategoryDto.Adapt(category));
//            await _unitOfWork.SaveChangeAsync();
//            return category.Adapt<SourceDTO>();
//        }
//    }
//}
