using Application.Commons;
using Application.DTO.Batches;
using Application.Interfaces;
using Application.IRepositories.Batches;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories.Batches
{
    public class BatchRepo : GenericRepository<Batch>, IBatchRepo
    {
        private readonly HypeCatDbContext _context;
        public BatchRepo(
            HypeCatDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
            ) :
            base(context, timeService, claimsService)
        {
            _context = context;
        }
        public async Task<Batch?> GetEarliestBatchWithDetailsByProductId(int productId)
        {
            return await _context.Batches
                .Include(b => b.BatchDetails)
                .Where(b => b.BatchDetails.Any(d => d.ProductId == productId))
                .OrderBy(b => b.CreateDate) // lấy batch được tạo sớm nhất
                .FirstOrDefaultAsync();
        }
        public async Task<Pagination<Batch>> GetPaginationAsync(BatchPaginationDTO paginationDTO)
        {
                    var query = _context.Batches
             .Include(b => b.BatchDetails)
        //.ThenInclude(bd => bd.SourceOfProduct)
    .Include(b => b.BatchDetails)
        .ThenInclude(bd => bd.Product)
             .AsQueryable();

            // Lọc theo IsDeleted
            if (paginationDTO.IsDeleted.HasValue)
            {
                query = query.Where(b => b.IsDeleted == paginationDTO.IsDeleted);
            }

            // Tìm kiếm theo SearchTerm (áp dụng cho các trường phù hợp, ví dụ: Code, Description,...)
            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                query = query.Where(b =>
                    b.CreateBy.Contains(paginationDTO.SearchTerm) ||
                    b.ModifiedBy.Contains(paginationDTO.SearchTerm));
            }

            // Sắp xếp theo trường được chỉ định
            if (!string.IsNullOrEmpty(paginationDTO.SortBy))
            {
                query = paginationDTO.SortBy.ToLower() switch
                {
                    "createdate" => paginationDTO.IsDescending ? query.OrderByDescending(b => b.CreateDate) : query.OrderBy(b => b.CreateDate),
                    "code" => paginationDTO.IsDescending ? query.OrderByDescending(b => b.CreateBy) : query.OrderBy(b => b.CreateBy),
                    "description" => paginationDTO.IsDescending ? query.OrderByDescending(b => b.ModifiedBy) : query.OrderBy(b => b.ModifiedBy),
                    "isdeleted" => paginationDTO.IsDescending ? query.OrderByDescending(b => b.IsDeleted) : query.OrderBy(b => b.IsDeleted),
                    _ => paginationDTO.IsDescending ? query.OrderByDescending(b => b.Id) : query.OrderBy(b => b.Id)
                };
            }
            else
            {
                query = paginationDTO.IsDescending ? query.OrderByDescending(b => b.Id) : query.OrderBy(b => b.Id);
            }

            // Tổng số bản ghi
            var totalCount = await query.CountAsync();

            // Phân trang
            var items = await query
                .Skip(paginationDTO.PageIndex * paginationDTO.PageSize)
                .Take(paginationDTO.PageSize)
                .ToListAsync();

            return new Pagination<Batch>
            {
                Items = items,
                TotalItemsCount = totalCount,
                PageSize = paginationDTO.PageSize,
                PageIndex = paginationDTO.PageIndex
            };
        }

        public async Task<List<Batch>> GetBatchesWithBatchDetailsByProductIdAsync(int productId)
        {
            var batches = await _context.Batches
                .Include(b => b.BatchDetails.Where(bd => bd.ProductId == productId))
                .Where(b => b.BatchDetails.Any(bd => bd.ProductId == productId))
                .ToListAsync();

            return batches;
        }
    }
}
