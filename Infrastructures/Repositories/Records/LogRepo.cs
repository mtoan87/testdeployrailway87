using Application.Commons;
using Application.DTO.Dashboards;
using Application.DTO.Orders;
using Application.DTO.Records;
using Application.Interfaces;
using Application.IRepositories.Records;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories.Records
{
    public class LogRepo : GenericRepository<Log>, ILogRepo
    {
        private readonly HypeCatDbContext _context;
        public LogRepo(
            HypeCatDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
            ) :
            base(context, timeService, claimsService)
        {
            _context = context;
        }
        public async Task<List<Log>> GetLogsByOrderIdsAsync(List<int> orderIds, string type = "Export")
        {
            return await _context.Logs
                .Where(log => orderIds.Contains(log.OrderId ?? 0) && log.Type == type)
                .ToListAsync();
        }
        public async Task<List<Log>> GetLogsByOrderAndProductAsync(int orderId, int productId, string logType)
        {
            return await _context.Logs
                .Where(l => l.OrderId == orderId && l.ProductId == productId && l.Type == logType)
                .ToListAsync();
        }
        public async Task<CustomerStatsDTO> GetCustomerStatsAsync()
        {
            var query = _context.Logs
                .Where(log => !string.IsNullOrEmpty(log.Phone))
                .GroupBy(log => log.Phone)
                .Select(group => new
                {
                    Types = group.Select(g => g.Type).Distinct().ToList()
                });

            var data = await query.ToListAsync();

            return new CustomerStatsDTO
            {
                TotalCustomer = data.Count,
                ImportCustomer = data.Count(x => x.Types.Contains("Import") && !x.Types.Contains("Export")),
                ExportCustomer = data.Count(x => x.Types.Contains("Export") && !x.Types.Contains("Import")),
                BothTypeCustomer = data.Count(x => x.Types.Contains("Import") && x.Types.Contains("Export"))
            };
        }
        public async Task<Pagination<Log>> GetPaginationAsync(LogPaginationDTO paginationDTO)
        {
            var query = _context.Logs
                .Include(p => p.Order)
                .ThenInclude(p => p.OrderDetails)
                .Include(p => p.Product)               
                .Include(p => p.User)
                .Include(p => p.BatchDetail)
                //.ThenInclude(p => p.SourceOfProduct)
                .AsQueryable();

            // Apply IsDeleted filter
            if (paginationDTO.IsDeleted.HasValue)
            {
                query = query.Where(p => p.IsDeleted == paginationDTO.IsDeleted);
            }
            if (!string.IsNullOrEmpty(paginationDTO.Type))
            {
                var type = paginationDTO.Type.ToLower();

                if (type == "stock")
                {
                    query = query.Where(p => p.Type == "Import" || p.Type == "Export");
                }
                else if (type == "price")
                {
                    query = query.Where(p => p.Type == "UpdatePrice");
                }
                else if (type == "expire")
                {
                    query = query.Where(p => p.Type == "Expired");
                }
                else if (type == "rollback")
                {
                    query = query.Where(p => p.Type == "Rollback");
                }
                else if (type == "pay")
                {
                    query = query.Where(p => p.Type == "Payment");
                }
            }
                // Apply search filters
                if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                var searchTerm = paginationDTO.SearchTerm.ToLower();

                query = query.Where(p =>
                                    p.Order.OrderDetails.Any(od => od.Name.ToLower().Contains(searchTerm)) ||
                                    p.Product.Name.ToLower().Contains(searchTerm) ||
                                    p.User.Name.ToLower().Contains(searchTerm)
                                    );
            }
            // Apply sorting
            if (!string.IsNullOrEmpty(paginationDTO.SortBy))
            {
                query = paginationDTO.SortBy.ToLower() switch
                {

                    "createdate" => paginationDTO.IsDescending ? query.OrderByDescending(p => p.CreateDate) : query.OrderBy(p => p.CreateDate),
                    "isdeleted" => paginationDTO.IsDescending ? query.OrderByDescending(p => p.IsDeleted) : query.OrderBy(p => p.IsDeleted),
                    _ => paginationDTO.IsDescending ? query.OrderByDescending(p => p.CreateDate) : query.OrderBy(p => p.CreateDate)
                };
            }
            else
            {
                query = paginationDTO.IsDescending ? query.OrderByDescending(p => p.CreateDate) : query.OrderBy(p => p.CreateDate);
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var items = await query
                .Skip(paginationDTO.PageIndex * paginationDTO.PageSize)
                .Take(paginationDTO.PageSize)
                .ToListAsync();

            return new Pagination<Log>
            {
                Items = items,
                TotalItemsCount = totalCount,
                PageSize = paginationDTO.PageSize,
                PageIndex = paginationDTO.PageIndex
            };
        }
    }
}
