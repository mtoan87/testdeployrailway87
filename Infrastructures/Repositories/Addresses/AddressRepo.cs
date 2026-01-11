using Application.Commons;
using Application.DTO.Addresses;
using Application.DTO.Categories;
using Application.Interfaces;
using Application.IRepositories.Addresses;
using Application.IRepositories.BatchDetails;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories.Addresses
{
    public class AddressRepo : GenericRepository<Address>, IAddressRepo
    {
        private readonly HypeCatDbContext _context;
        private readonly IClaimsService _claimsService;
        private readonly ICurrentTime _timeService;
        public AddressRepo(
            HypeCatDbContext context,
            ICurrentTime timeService,       
            IClaimsService claimsService
            ) :
            base(context, timeService, claimsService)
        {
            _timeService = timeService;
            _context = context;
            _claimsService = claimsService;
        }
        public async Task<Address?> GetAddressByAddressIdAndUserId(int addressId)
        {
            return await _context.Addresses
             
                .FirstOrDefaultAsync(c =>
                    c.UserId == _claimsService.GetCurrentUserId &&
                    c.Id == addressId &&
                    (c.IsDeleted == false));
        }

        public async Task<List<Address>> GetListAddressByUserIdAsync()
        {
            return await _context.Addresses
                .Where(a => a.UserId == _claimsService.GetCurrentUserId && (a.IsDeleted == false || a.IsDeleted == null))
                .ToListAsync();
        }

        public async Task<Pagination<Address>> GetPaginationAsync(AddressPaginationDTO paginationDTO)
        {
            var query = _context.Addresses.AsQueryable();

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
                    (!string.IsNullOrEmpty(c.District) && c.District.ToLower().Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(c.Province) && c.Province.ToLower().Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(c.Ward) && c.Ward.ToLower().Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(c.Street) && c.Street.ToLower().Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(c.RecipientName) && c.RecipientName.ToLower().Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(c.Phone) && c.Phone.ToLower().Contains(searchTerm))
                );
            }



            // Sắp xếp
            if (!string.IsNullOrEmpty(paginationDTO.SortBy))
            {
                query = paginationDTO.SortBy.ToLower() switch
                {
                    "isdefault" => paginationDTO.IsDescending ? query.OrderByDescending(c => c.IsDefault) : query.OrderBy(c => c.IsDefault),
                    "ward" => paginationDTO.IsDescending ? query.OrderByDescending(c => c.Ward) : query.OrderBy(c => c.Ward),
                    "district" => paginationDTO.IsDescending ? query.OrderByDescending(c => c.District) : query.OrderBy(c => c.District),
                    "street" => paginationDTO.IsDescending ? query.OrderByDescending(c => c.Street) : query.OrderBy(c => c.Street),
                    "phone" => paginationDTO.IsDescending ? query.OrderByDescending(c => c.Phone) : query.OrderBy(c => c.Phone),
                    "id" => paginationDTO.IsDescending ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                    "name" => paginationDTO.IsDescending ? query.OrderByDescending(c => c.RecipientName) : query.OrderBy(c => c.RecipientName),
                    "province" => paginationDTO.IsDescending ? query.OrderByDescending(c => c.Province) : query.OrderBy(c => c.Province),
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

            return new Pagination<Address>
            {
                Items = items,
                TotalItemsCount = totalCount,
                PageSize = paginationDTO.PageSize,
                PageIndex = paginationDTO.PageIndex
            };
        }
    }
}
