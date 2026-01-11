using Application.Commons;
using Application.DTO.BatchDetails;
using Application.DTO.Batches;
using Application.DTO.Categories;
using Application.DTO.Orders;
using Application.DTO.Products;
using Application.DTO.SourceOfProducts;
using Application.Interfaces;
using Application.Interfaces.Batches;
using Domain.Enum;
using Domain.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Batches
{
    public class BatchService : IBatchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentTime _currentTime;
        public BatchService(IUnitOfWork unitOfWork, ICurrentTime currentTime)
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
        }

        public async  Task<Batch?> GetEarliestBatchWithDetailsByProductId(int productId)
        {
            return await _unitOfWork.BatchRepo.GetEarliestBatchWithDetailsByProductId(productId);
        }
        public async Task<BatchDTO> CreateBatchAsync(CreateBatchDTO  request)
        {
            if (request.BatchDetailsDTO == null || !request.BatchDetailsDTO.Any())
                throw new Exception("Batch must have at least one product.");

            var productIds = request.BatchDetailsDTO.Select(d => d.ProductId).ToList();
            var products = await _unitOfWork.ProductRepo.GetByIdsAsync(productIds);

            var order = new Batch
            {
                CreateDate = _currentTime.GetCurrentTime(),
            };
            await _unitOfWork.BatchRepo.AddAsync(order);
            await _unitOfWork.SaveChangeAsync();

            var orderDetails = request.BatchDetailsDTO.Select(detail => new BatchDetail
            {
                BatchId = order.Id,
                ProductId = detail.ProductId,
                Quantity = detail.Quantity,
                RemainingQuantity = detail.Quantity,
                SellingPrice = detail.SellingPrice,
                ImportCosts = detail.ImportCosts,
            }).ToList();

            await _unitOfWork.BatchDetailRepo.AddRangeAsync(orderDetails);
            await _unitOfWork.SaveChangeAsync();

            // ✅ Tạo log cho mỗi sản phẩm nhập vào batch
            var logs = orderDetails.Select(d => new Log
            {
                ProductId = d.ProductId,
                Quantity = d.Quantity,
                Type = "Import",
                OldImportCost = d.ImportCosts,
                OldSellingPrice = d.SellingPrice,
                BatchDetailId = d.Id,
                BatchId = d.BatchId
            }).ToList();

            await _unitOfWork.LogRepo.AddRangeAsync(logs);
            await _unitOfWork.SaveChangeAsync();

            return order.Adapt<BatchDTO>();
        }
        public async Task<BatchDTO> UpdateBatch(int id, UpdateBatchDTO updateCategoryDto)
        {
            var category = await _unitOfWork.BatchRepo.GetByIdAsync(id);
            if (category is null)
            {
                throw new Exception("Batch does not exist.");
            }



            updateCategoryDto.Adapt(category); // Gán các giá trị mới vào category
            _unitOfWork.BatchRepo.Update(category);
            await _unitOfWork.SaveChangeAsync();
            return category.Adapt<BatchDTO>();
        }
        public async Task DeleteOrEnable(int categoryId, bool isDeleted)
        {
            var category = await _unitOfWork.BatchRepo.GetAsync(d => d.Id == categoryId);
            if (category is null)
            {
                throw new Exception("Batch does not exist.");
            }
            category.IsDeleted = isDeleted;
            await _unitOfWork.SaveChangeAsync();
        }
        public async Task<Pagination<BatchDTO>> GetPaginationAsync(BatchPaginationDTO paginationDTO)
        {
            try
            {
                var pagination = await _unitOfWork.BatchRepo.GetPaginationAsync(paginationDTO);

                return new Pagination<BatchDTO>
                {
                    Items = pagination.Items.Adapt<List<BatchDTO>>(),
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

        public async Task<BatchWDetailDTO?> GetBatchById(int id)
        {
            var batch = await _unitOfWork.BatchRepo.GetById(
                id,
                includeProperties: "BatchDetails,BatchDetails.Product,BatchDetails.Product.Category"
            );

            if (batch is null)
            {
                throw new Exception("Batch is not existed");
            }

            // Chuyển từng BatchDetail thành DTO
            var detailDTOs = batch.BatchDetails.Select(d => new BatchDetailDTO
            {
                Id = d.Id,
                BatchId = d.BatchId,
                ProductId = d.ProductId,
                SellingPrice = d.SellingPrice,
                ImportCosts = d.ImportCosts,
                RemainingQuantity = d.RemainingQuantity,
                Quantity = d.Quantity,
                BatchDetailParentId = d.BatchDetailParentId,
                ProductDTO = d.Product == null ? null! : new ProductOrderDTO
                {
                    Id = d.Product.Id,
                    Name = d.Product.Name,
                    CategoryDTO = d.Product.Category == null ? null! : new CategoryProductDTO
                    {
                        Id = d.Product.Category.Id,
                        Name = d.Product.Category.Name
                    }
                }
            }).ToList();

            // Lọc cha và nhóm con theo cha
            var groupList = detailDTOs
                .Where(d => d.BatchDetailParentId == null) // parent
                .Select(parent => new BatchDetailGroupDTO
                {
                    BatchdetailParent = parent,
                    BatchdetailChild = detailDTOs
                        .Where(child => child.BatchDetailParentId == parent.Id)
                        .ToList()
                })
                .ToList();

            return new BatchWDetailDTO
            {
                Id = batch.Id,
                CreateDate = batch.CreateDate,
                BatchDetailDTOs = groupList
            };
        }
    }
}
