using Application.Commons;
using Application.DTO.BatchDetails;
using Application.DTO.Batches;
using Application.DTO.Users;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.BatchDetails
{
    public interface IBatchDetailService
    {
        Task<Pagination<BatchDetailDTO>> GetPaginationAsync(BatchPaginationDTO paginationDTO);
        Task DeleteOrEnable(int categoryId, bool isDeleted);
        Task<BatchDetailDTO> UpdateBatch(int id, UpdateBatchDetailDTO updateCategoryDto);
        Task<List<BatchDetail>> GetByProductIdsAsync(List<int> productIds);
        Task<bool> UpdateBatchStock(int productId, int quantity, string transactionType, UserInforDTO userInfor);
    }
}
