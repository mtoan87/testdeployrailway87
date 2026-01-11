using Application.Commons;
using Application.DTO.Addresses;
using Application.DTO.IntroImages;
using Application.Interfaces;
using Application.IRepositories.IntroImages;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories.IntroImages
{
    public class IntroImageRepo : GenericRepository<IntroImage>, IIntroImageRepo
    {
        private readonly HypeCatDbContext _context;
        public IntroImageRepo(
          HypeCatDbContext context,
          ICurrentTime timeService,
          IClaimsService claimsService
          ) :
          base(context, timeService, claimsService)
        {
            _context = context;
        }


        public async Task<Pagination<IntroImage>> GetPaginationAsync(IntroImagePagination paginationDTO)
        {
            var query = _context.IntroImages.AsQueryable();

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
                    (!string.IsNullOrEmpty(c.CreateBy) && c.CreateBy.ToLower().Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(c.ModifiedBy) && c.ModifiedBy.ToLower().Contains(searchTerm)) 
                   
                );
            }



            // Sắp xếp
            if (!string.IsNullOrEmpty(paginationDTO.SortBy))
            {
                query = paginationDTO.SortBy.ToLower() switch
                {
                    "createby" => paginationDTO.IsDescending ? query.OrderByDescending(c => c.CreateBy) : query.OrderBy(c => c.CreateBy),
                    "modify" => paginationDTO.IsDescending ? query.OrderByDescending(c => c.ModifiedBy) : query.OrderBy(c => c.ModifiedBy),                   
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

            return new Pagination<IntroImage>
            {
                Items = items,
                TotalItemsCount = totalCount,
                PageSize = paginationDTO.PageSize,
                PageIndex = paginationDTO.PageIndex
            };
        }
    }
}
