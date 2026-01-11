using Application.Commons;
using Application.DTO.BatchDetails;
using Application.DTO.Batches;
using Application.Interfaces;
using Application.IRepositories.BatchDetails;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories.BatchDetails
{
    public class BatchDetailRepo : GenericRepository<BatchDetail>, IBatchDetailRepo
    {
        private readonly HypeCatDbContext _context;
        public BatchDetailRepo(
            HypeCatDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
            ) :
            base(context, timeService, claimsService)
        {
            _context = context;
        }
        public async Task<List<BatchDetail>> GetFirstValidBatchDetails(int productId)
        {
            var batchDetails = await _context.BatchDetails
                .Include(bd => bd.Batch)
                .Where(bd => bd.ProductId == productId && bd.RemainingQuantity > 0)
                .ToListAsync();

            // Nhóm theo BatchId và lấy CreatedDate đúng theo từng batch
            var validBatches = batchDetails
                .GroupBy(bd => bd.BatchId)
                .Select(g => new
                {
                    BatchId = g.Key,
                    CreatedDate = g.First().Batch.CreateDate, // vì đã Include Batch nên dùng được
                    TotalRemaining = g.Sum(x => x.RemainingQuantity ?? 0),
                    Details = g.ToList()
                })
                .Where(x => x.TotalRemaining > 0) // chỉ giữ lại những batch còn hàng
                .OrderBy(x => x.CreatedDate) // ưu tiên batch nhập sớm
                .FirstOrDefault();

            return validBatches?.Details ?? new List<BatchDetail>();
        }
        public async Task<List<BatchDetailUserProductDTO>> GetBatchDetailsByProductIdAsync(int productId)
        {
            var batchDetails = await _context.BatchDetails
                .Include(bd => bd.Product)
                .Include(bd => bd.Batch)
                .Where(bd => bd.ProductId == productId && bd.RemainingQuantity > 0)
                .OrderBy(bd => bd.Batch.CreateDate)
                .ToListAsync();

            var earliestBatchId = batchDetails
                .Select(bd => bd.BatchId)
                .FirstOrDefault();

            var targetDetails = batchDetails
                .Where(bd => bd.BatchId == earliestBatchId)
                .ToList();

            var totalRemaining = targetDetails.Sum(bd => bd.RemainingQuantity ?? 0);

            return targetDetails.Select(bd => new BatchDetailUserProductDTO
            {
                Id = bd.Id,
                SellingPrice = bd.SellingPrice,
                RemainingQuantity = totalRemaining,              
            }).ToList();
        }
        public async Task<Pagination<BatchDetail>> GetPaginationAsync(BatchPaginationDTO paginationDTO)
        {
            try
            {
                var query = _context.BatchDetails
                    .Include(bd => bd.Product)
                    //.Include(bd => bd.SourceOfProduct)
                    .Include(bd => bd.Batch)
                    .AsQueryable();

                // Lọc theo IsDeleted
                if (paginationDTO.IsDeleted.HasValue)
                {
                    query = query.Where(bd => bd.IsDeleted == paginationDTO.IsDeleted);
                }

                // Tìm kiếm an toàn
                if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
                {
                    var searchTerm = paginationDTO.SearchTerm.ToLower();

                    query = query.Where(bd =>
                        (bd.Product != null && bd.Product.Name != null && bd.Product.Name.ToLower().Contains(searchTerm)) ||
                        (!string.IsNullOrEmpty(bd.CreateBy) && bd.CreateBy.ToLower().Contains(searchTerm)));
                }

                // Sắp xếp
                if (!string.IsNullOrEmpty(paginationDTO.SortBy))
                {
                    query = paginationDTO.SortBy.ToLower() switch
                    {
                        "quantity" => paginationDTO.IsDescending ? query.OrderByDescending(bd => bd.Quantity) : query.OrderBy(bd => bd.Quantity),
                        "expireddate" => paginationDTO.IsDescending ? query.OrderByDescending(bd => bd.ExpiredDate) : query.OrderBy(bd => bd.ExpiredDate),
                        "createdate" => paginationDTO.IsDescending ? query.OrderByDescending(bd => bd.CreateDate) : query.OrderBy(bd => bd.CreateDate),
                        "createby" => paginationDTO.IsDescending ? query.OrderByDescending(bd => bd.CreateBy) : query.OrderBy(bd => bd.CreateBy),
                        _ => paginationDTO.IsDescending ? query.OrderByDescending(bd => bd.Id) : query.OrderBy(bd => bd.Id)
                    };
                }
                else
                {
                    query = paginationDTO.IsDescending ? query.OrderByDescending(bd => bd.Id) : query.OrderBy(bd => bd.Id);
                }

                var totalCount = await query.CountAsync();

                var items = await query
                    .Skip(paginationDTO.PageIndex * paginationDTO.PageSize)
                    .Take(paginationDTO.PageSize)
                    .ToListAsync();

                return new Pagination<BatchDetail>
                {
                    Items = items,
                    TotalItemsCount = totalCount,
                    PageSize = paginationDTO.PageSize,
                    PageIndex = paginationDTO.PageIndex
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting pagination: {ex.Message}", ex);
            }
        }

        public async Task<List<BatchDetail>> GetByProductIdsAsync(List<int> productIds)
        {
            return await _context.BatchDetails
                .Where(b => productIds.Contains(b.ProductId) && b.IsExpiredLogged == false)
                .ToListAsync();
        }

        public async Task<BatchDetail?> GetFirstAvailableBatch(int productId)
        {
            return await _context.BatchDetails
                .Where(b => b.ProductId == productId && b.RemainingQuantity > 0 && (b.IsDeleted == false || b.IsDeleted == null))
                .OrderBy(b => b.CreateDate)
                .FirstOrDefaultAsync();
        }

        public async Task<List<BatchDetail>> GetAvailableBatches(int productId)
        {
            return await _context.BatchDetails
                .Where(b =>
                    b.ProductId == productId &&
                    (b.IsDeleted == null || b.IsDeleted == false) &&
                    (b.RemainingQuantity ?? 0) > 0)
                .OrderBy(b => b.CreateDate) // FIFO: lô nào tạo sớm thì ưu tiên
                .ToListAsync();
        }
    }
}
