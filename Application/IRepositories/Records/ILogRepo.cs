using Application.Commons;
using Application.DTO.Dashboards;
using Application.DTO.Records;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories.Records
{
    public interface ILogRepo : IGenericRepository<Log>
    {
        Task<Pagination<Log>> GetPaginationAsync(LogPaginationDTO paginationDTO);
        Task<CustomerStatsDTO> GetCustomerStatsAsync();
        Task<List<Log>> GetLogsByOrderIdsAsync(List<int> orderIds, string type = "Export");

        Task<List<Log>> GetLogsByOrderAndProductAsync(int orderId, int productId, string logType);
    }
}
