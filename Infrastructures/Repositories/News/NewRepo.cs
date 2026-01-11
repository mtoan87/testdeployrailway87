using Application.Commons;
using Application.DTO.Addresses;
using Application.DTO.News;
using Application.Interfaces;
using Application.IRepositories.News;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories.News
{
    public class NewRepo : GenericRepository<Domain.Model.News>, INewRepo
    {
        private readonly HypeCatDbContext _context;
        public NewRepo(
           HypeCatDbContext context,
           ICurrentTime timeService,
           IClaimsService claimsService
           ) :
           base(context, timeService, claimsService)
        {
            _context = context;
        }
        public async Task<Pagination<Domain.Model.News>> GetPaginationAsync(NewPaginationDTO paginationDTO)
        {
            var query = _context.News
                .Include(n =>n.Images)
                .Include(n => n.Category)
                .AsQueryable();

            // Lọc theo IsDeleted
            if (paginationDTO.IsDeleted.HasValue)
            {
                query = query.Where(c => c.IsDeleted == paginationDTO.IsDeleted);
            }

            // Tìm kiếm theo SearchTerm
            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                var searchTerm = paginationDTO.SearchTerm.ToLower();

                query = query.Where(c =>
                    (!string.IsNullOrEmpty(c.Title) && c.Title.ToLower().Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(c.Content) && c.Content.ToLower().Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(c.Writer) && c.Writer.ToLower().Contains(searchTerm))
                  
                );
            }
           // Sắp xếp
            if (!string.IsNullOrEmpty(paginationDTO.SortBy))
            {
                query = paginationDTO.SortBy.ToLower() switch
                {                  
                    "writer" => paginationDTO.IsDescending ? query.OrderByDescending(c => c.Writer) : query.OrderBy(c => c.Writer),
                    "title" => paginationDTO.IsDescending ? query.OrderByDescending(c => c.Title) : query.OrderBy(c => c.Title),
                    "content" => paginationDTO.IsDescending ? query.OrderByDescending(c => c.Content) : query.OrderBy(c => c.Content),
                    "isdeleted" => paginationDTO.IsDescending ? query.OrderByDescending(c => c.IsDeleted) : query.OrderBy(c => c.IsDeleted),
                    _ => paginationDTO.IsDescending ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id)
                };
            }
            else
            {
                query = paginationDTO.IsDescending ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id);
            }

            // Tổng số bản ghi
            var totalCount = await query.CountAsync();

            // Áp dụng phân trang
            var items = await query
                .Skip(paginationDTO.PageIndex * paginationDTO.PageSize)
                .Take(paginationDTO.PageSize)
                .ToListAsync();

            return new Pagination<Domain.Model.News>
            {
                Items = items,
                TotalItemsCount = totalCount,
                PageSize = paginationDTO.PageSize,
                PageIndex = paginationDTO.PageIndex
            };
        }
    }
}
