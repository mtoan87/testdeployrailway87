
using Application.DTO.Addresses;
using Application.DTO.News;
using Application.Interfaces.News;
using Microsoft.AspNetCore.Mvc;

namespace NgocBichKiot.Api.Controllers.News
{
    public class NewController : BaseController
    {
        private readonly INewService _service;
        public NewController(INewService newService)
        {
            _service = newService;  
        }

        [HttpGet]

        public async Task<IActionResult> GetNewsPagination([FromQuery] NewPaginationDTO paginationDTO)
        {
            try
            {
                var result = await _service.GetPaginationAsync(paginationDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddNew([FromBody] CreateNewDTO createNewDTO)
        {
            try
            {
                var result = await _service.AddNew(createNewDTO);
                return Created(nameof(createNewDTO), result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNew(int id, [FromBody] UpdateNewDTO update)
        {

            try
            {
                var result = await _service.UpdateNew(id, update);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPut("{newId}/{isDeleted}")]
        public async Task<IActionResult> DeleteOrEnable(int newId, int isDeleted)
        {


            try
            {
                await _service.DeleteOrEnable(newId, isDeleted > 0);
                return NoContent();
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = ex.Message });
            }

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNewById(int id)
        {
            var result = await _service.GetNewById(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNews()
        {
            var result = await _service.GetAllNews();
            return Ok(result);
        }
    }
}
