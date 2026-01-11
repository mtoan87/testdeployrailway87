using Application.Commons;
using Application.DTO.BatchDetails;
using Application.DTO.Batches;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Batches
{
    public interface IBatchService
    {
        Task<Batch?> GetEarliestBatchWithDetailsByProductId(int productId);
        Task<Pagination<BatchDTO>> GetPaginationAsync(BatchPaginationDTO paginationDTO);
        Task<BatchWDetailDTO?> GetBatchById(int id);
        Task<BatchDTO> CreateBatchAsync(CreateBatchDTO request);
        Task<BatchDTO> UpdateBatch(int id, UpdateBatchDTO updateCategoryDto);
        Task DeleteOrEnable(int categoryId, bool isDeleted);
    }
}
