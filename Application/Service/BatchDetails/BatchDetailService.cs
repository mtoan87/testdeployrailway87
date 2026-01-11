using Application.Commons;
using Application.DTO.BatchDetails;
using Application.DTO.Batches;
using Application.DTO.Categories;
using Application.DTO.Users;
using Application.Interfaces;
using Application.Interfaces.BatchDetails;
using Domain.Enum;
using Domain.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.BatchDetails
{
    public class BatchDetailService : IBatchDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BatchDetailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Pagination<BatchDetailDTO>> GetPaginationAsync(BatchPaginationDTO paginationDTO)
        {
            try
            {
                var pagination = await _unitOfWork.BatchDetailRepo.GetPaginationAsync(paginationDTO);

                return new Pagination<BatchDetailDTO>
                {
                    
                    Items = pagination.Items.Adapt<List<BatchDetailDTO>>(),
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
        public async Task<List<BatchDetail>> GetByProductIdsAsync(List<int> productIds)
        {
            return await _unitOfWork.BatchDetailRepo.GetByProductIdsAsync(productIds);
        }
        public async Task DeleteOrEnable(int categoryId, bool isDeleted)
        {
            var category = await _unitOfWork.BatchDetailRepo.GetAsync(d => d.Id == categoryId);
            if (category is null)
            {
                throw new Exception("BatchDetail does not exist.");
            }
            category.IsDeleted = isDeleted;
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task<bool> UpdateBatchStock(int productId, int quantity, string transactionType, UserInforDTO userInfor)
        {
            // 1️⃣ Lấy sản phẩm từ DB
            var product = await _unitOfWork.BatchDetailRepo.GetByIdAsync(productId);
            if (product == null)
            {
                throw new Exception("Không tìm thấy gói nhập.");
            }


            if (transactionType == LogType.Import.ToString()) // Nhập hàng
            {
                product.RemainingQuantity += quantity;
            }
            else if (transactionType == LogType.Export.ToString()) // Xuất hàng
            {
                if (product.RemainingQuantity < quantity)
                {
                    throw new Exception("Không đủ số lượng tồn kho!.");
                }
                product.RemainingQuantity -= quantity;
            }
            else
            {
                throw new Exception("Invalid transaction type.");
            }

            // 3️⃣ Cập nhật lại sản phẩm
            _unitOfWork.BatchDetailRepo.Update(product);
            await _unitOfWork.SaveChangeAsync();

            // 4️⃣ Ghi Log lịch sử
            var log = new Log
            {
                //ProductId = product.Id,
                Name = userInfor.Name,
                Phone = userInfor.Phone,
                Address = userInfor.Address,
                //UserId = _claimsService.GetCurrentUserId,
                Quantity = quantity,
                Type = transactionType,

            };

            await _unitOfWork.LogRepo.AddAsync(log);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }

        public async Task<BatchDetailDTO> UpdateBatch(int id, UpdateBatchDetailDTO updateCategoryDto)
        {
            var category = await _unitOfWork.BatchDetailRepo.GetByIdAsync(id);
            if (category is null)
            {
                throw new Exception("BatchDetail does not exist.");
            }

            

            updateCategoryDto.Adapt(category); // Gán các giá trị mới vào category
            _unitOfWork.BatchDetailRepo.Update(category);
            await _unitOfWork.SaveChangeAsync();
            var log = new Log
            {
                BatchId = category.BatchId,
                BatchDetailId = category.Id,
                ProductId = category.ProductId,
                OldSellingPrice = category.SellingPrice,
                NewSellingPrice = updateCategoryDto.SellingPrice,
                Type = LogType.UpdatePrice.ToString(),

            };

            await _unitOfWork.LogRepo.AddAsync(log);
            await _unitOfWork.SaveChangeAsync();
            return category.Adapt<BatchDetailDTO>();
        }
    }
}
