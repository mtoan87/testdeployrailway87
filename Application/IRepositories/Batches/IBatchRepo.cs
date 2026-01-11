using Application.Commons;
using Application.DTO.Batches;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories.Batches
{
    public interface IBatchRepo : IGenericRepository<Batch>
    {
        Task<Batch?> GetEarliestBatchWithDetailsByProductId(int productId);
        Task<List<Batch>> GetBatchesWithBatchDetailsByProductIdAsync(int productId);
        Task<Pagination<Batch>> GetPaginationAsync(BatchPaginationDTO paginationDTO);
    }
}
