using Application.DTO.Addresses;
using Application.DTO.Categories;
using Application.Interfaces.Addresses;
using Microsoft.AspNetCore.Mvc;

namespace NgocBichKiot.Api.Controllers.Addresses
{
    public class AddressController : BaseController
    {
        private readonly IAddressService _service;
        public AddressController(IAddressService service)
        {
            _service = service;
        }

        [HttpGet]

        public async Task<IActionResult> GetAddressPagination([FromQuery] AddressPaginationDTO paginationDTO)
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
        public async Task<IActionResult> CreateAddress([FromBody] CreateAddressDTO createAddress)
        {
            
                var result = await _service.AddAddress(createAddress);
                return Created(nameof(CreateAddress), result);
            
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] UpdateAddressDTO updateAddress)
        {

            try
            {
                var result = await _service.UpdateAddress(id, updateAddress);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPut("{addressId}/{isDeleted}")]
        public async Task<IActionResult> DeleteOrEnable(int addressId, int isDeleted)
        {


            try
            {
                await _service.DeleteOrEnable(addressId, isDeleted > 0);
                return NoContent();
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = ex.Message });
            }

        }
        [HttpPut()]
        public async Task<IActionResult> DefaultOrNot(int addressId, int isDefault, int userId)
        {


            try
            {
                await _service.DefaultOrNot(addressId, isDefault > 0, userId);
                return NoContent();
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = ex.Message });
            }

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddressBy(int id)
        {
            var result = await _service.GetAddressById(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAddresses()
        {
            var result = await _service.GetAllAddresses();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAddressByUserId()
        {
            var result = await _service.GetAddressByUserId();
            return Ok(result);
        }
    }
}
