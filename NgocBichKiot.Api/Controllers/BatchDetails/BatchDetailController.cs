using Application.DTO.BatchDetails;
using Application.DTO.Batches;
using Application.DTO.Products;
using Application.DTO.Users;
using Application.Interfaces.BatchDetails;
using Microsoft.AspNetCore.Mvc;

namespace NgocBichKiot.Api.Controllers.BatchDetails
{
    public class BatchDetailController : BaseController
    {
        private readonly IBatchDetailService service;
        public BatchDetailController(IBatchDetailService _service)
        {
            service = _service;
        }
        [HttpGet]

        public async Task<IActionResult> GetBatchDetailPagination([FromQuery] BatchPaginationDTO paginationDTO)
        {
            try
            {
                var result = await service.GetPaginationAsync(paginationDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBatchDetail(int id, [FromBody] UpdateBatchDetailDTO accountDTO)
        {
            try
            {
                var updatedProd = await service.UpdateBatch(id, accountDTO);
                return Ok(updatedProd);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPost()]
        public async Task<IActionResult> GetByProductIdsAsync(List<int> productIds)
        {
            
                var updatedProd = await service.GetByProductIdsAsync(productIds);
                return Ok(updatedProd);
            
        }
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateProductQuantity(int id, [FromBody] UpdateProductQuantity accountDTO)
        //{
        //    var updatedProd = await _productService.UpdateProductQuantityAsync(id, accountDTO);
        //    return Ok(updatedProd);
        //}
        [HttpPut()]
        public async Task<IActionResult> UpdateBatchStock(int batchId, int quantity, string type, UserInforDTO userInfor)
        {

            
                var updatedProd = await service.UpdateBatchStock(batchId, quantity, type, userInfor);
                return Ok(updatedProd);
            
            
        }

        [HttpPut("{bacthDetailId}/{isDeleted}")]
        public async Task<IActionResult> DeleteOrEnable(int bacthDetailId, int isDeleted)
        {

            try
            {
                await service.DeleteOrEnable(bacthDetailId, isDeleted > 0);
                return NoContent();
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = ex.Message });
            }

        }
    }
}
