using Application.DTO.Categories;
using Application.DTO.Orders;
using Application.DTO.Products;
using Application.DTO.Records;
using Application.Interfaces.Records;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NgocBichKiot.Api.Services.Examples;
using Swashbuckle.AspNetCore.Filters;

namespace NgocBichKiot.Api.Controllers.Records
{
    
    public class LogsController : BaseController
    {
        private readonly IRecordService _recordService;
       
        public LogsController(IRecordService record)
        {
            _recordService = record;
            
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<LogListDTO>), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GetListLogDTOExample))]
        public async Task<IActionResult> GetLogPagination([FromQuery] LogPaginationDTO paginationDTO)
        {
            try
            {
                var result = await _recordService.GetPaginationAsync(paginationDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
          
        }
    }
}
