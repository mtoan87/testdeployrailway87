using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Model;
using System.Threading.Tasks;
using Application.Commons;
using Application.DTO.Batches;
using Application.DTO.BatchDetails;

namespace Application.IRepositories.BatchDetails
{
    public interface IBatchDetailRepo : IGenericRepository<BatchDetail>
    {
        Task<List<BatchDetailUserProductDTO>> GetBatchDetailsByProductIdAsync(int productId);
        Task<List<BatchDetail>> GetFirstValidBatchDetails(int productId);
        Task<Pagination<BatchDetail>> GetPaginationAsync(BatchPaginationDTO paginationDTO);
        Task<List<BatchDetail>> GetByProductIdsAsync(List<int> productIds);
        Task<List<BatchDetail>> GetAvailableBatches(int productId);
        Task<BatchDetail?> GetFirstAvailableBatch(int productId);
    }
}
