//using Application.Commons;
//using Application.DTO.Categories;
//using Application.Interfaces;
//using Application.IRepositories.Categories;
//using Application.IRepositories.SourceOfProducts;
//using Domain.Model;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Infrastructures.Repositories.SourceOfProducts
//{
//    public class SourceRepo : GenericRepository<SourceOfProduct>, ISourceRepo
//    {
//        private readonly NgocBichKiotDbContext _context;
//        public SourceRepo(
//            NgocBichKiotDbContext context,
//            ICurrentTime timeService,
//            IClaimsService claimsService
//            ) :
//            base(context, timeService, claimsService)
//        {
//            _context = context;
//        }

//        public async Task<Pagination<SourceOfProduct>> GetPaginationAsync(CategoryPaginationDTO paginationDTO)
//        {
//            var query = _context.SourceOfProducts.AsQueryable();

//            // Lọc theo IsDeleted
//            if (paginationDTO.IsDeleted.HasValue)
//            {
//                query = query.Where(c => c.IsDeleted == paginationDTO.IsDeleted);
//            }

//            // Tìm kiếm theo SearchTerm
//            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
//            {
//                query = query.Where(c =>
//                    c.Name.Contains(paginationDTO.SearchTerm));
//            }

//            // Lọc theo Name cụ thể
//            if (!string.IsNullOrEmpty(paginationDTO.Name))
//            {
//                query = query.Where(c => c.Name.Contains(paginationDTO.Name));
//            }

//            // Sắp xếp
//            if (!string.IsNullOrEmpty(paginationDTO.SortBy))
//            {
//                query = paginationDTO.SortBy.ToLower() switch
//                {
//                    "name" => paginationDTO.IsDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
//                    "isdeleted" => paginationDTO.IsDescending ? query.OrderByDescending(c => c.IsDeleted) : query.OrderBy(c => c.IsDeleted),
//                    _ => paginationDTO.IsDescending ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id)
//                };
//            }
//            else
//            {
//                query = paginationDTO.IsDescending ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id);
//            }

//            // Tổng số bản ghi
//            var totalCount = await query.CountAsync();

//            // Áp dụng phân trang
//            var items = await query
//                .Skip(paginationDTO.PageIndex * paginationDTO.PageSize)
//                .Take(paginationDTO.PageSize)
//                .ToListAsync();

//            return new Pagination<SourceOfProduct>
//            {
//                Items = items,
//                TotalItemsCount = totalCount,
//                PageSize = paginationDTO.PageSize,
//                PageIndex = paginationDTO.PageIndex
//            };
//        }
//    }
//}
