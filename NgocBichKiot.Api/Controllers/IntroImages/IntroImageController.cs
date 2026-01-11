using Application.DTO.IntroImages;
using Application.DTO.News;
using Application.Interfaces.IntroImages;
using Microsoft.AspNetCore.Mvc;

namespace NgocBichKiot.Api.Controllers.IntroImages
{
    public class IntroImageController : BaseController
    {
        private readonly IIntroImageService _service;
        public IntroImageController(IIntroImageService service)
        {
            _service = service;
        }

        [HttpGet]

        public async Task<IActionResult> GetCategoryPagination([FromQuery] IntroImagePagination paginationDTO)
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
        public async Task<IActionResult> AddIntroImage([FromBody] CreateIntroImage create)
        {
            try
            {
                var result = await _service.AddIntroImage(create);
                return Created(nameof(create), result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIntroImage(int id, [FromBody] UpdateIntroImage update)
        {

            try
            {
                var result = await _service.UpdateIntroImage(id, update);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPut("{categoryId}/{isDeleted}")]
        public async Task<IActionResult> DeleteOrEnable(int imdId, int isDeleted)
        {


            try
            {
                await _service.DeleteOrEnable(imdId, isDeleted > 0);
                return NoContent();
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = ex.Message });
            }

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIntroImageById(int id)
        {
            var result = await _service.GetIntroImageById(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllIntroImages()
        {
            var result = await _service.GetAllIntroImages();
            return Ok(result);
        }
    }
}
