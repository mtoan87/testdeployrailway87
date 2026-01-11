using Application.DTO.BatchDetails;
using Application.DTO.Batches;
using Application.DTO.Categories;
using Application.DTO.Orders;
using Application.Interfaces.Batches;
using Microsoft.AspNetCore.Mvc;

namespace NgocBichKiot.Api.Controllers.Batches
{
    public class BatchController : BaseController
    {
        private readonly IBatchService _batchService;
        public BatchController(IBatchService batchService)
        {
            _batchService = batchService;
        }
        [HttpGet]

        public async Task<IActionResult> GetBatchPagination([FromQuery] BatchPaginationDTO paginationDTO)
        {
            try
            {
                var result = await _batchService.GetPaginationAsync(paginationDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBatchById(int id)
        {
            var result = await _batchService.GetBatchById(id);
            return Ok(result);
        }
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetBatchWithdDetailByProductId(int productId)
        {
            var result = await _batchService.GetEarliestBatchWithDetailsByProductId(productId);
            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> CreateBatch([FromBody] CreateBatchDTO createProduct)
        {
            var response = await _batchService.CreateBatchAsync(createProduct);
            return Created(nameof(CreateBatch), response);
            //try
            //{
                
            //}
            //catch (Exception ex)
            //{
            //    return StatusCode(500, new { message = ex.Message });
            //}
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBatch(int id, [FromBody] UpdateBatchDTO accountDTO)
        {
            try
            {
                var updatedProd = await _batchService.UpdateBatch(id, accountDTO);
                return Ok(updatedProd);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPut("{bacthId}/{isDeleted}")]
        public async Task<IActionResult> DeleteOrEnable(int bacthId, int isDeleted)
        {

            try
            {
                await _batchService.DeleteOrEnable(bacthId, isDeleted > 0);
                return NoContent();
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = ex.Message });
            }

        }
    }
}
