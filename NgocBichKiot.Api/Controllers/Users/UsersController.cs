using Application.DTO.OrderDetails;
using Application.DTO.Orders;
using Application.DTO.Users;
using Application.Interfaces.Users;
using Application.Service.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NgocBichKiot.Api.Services.Examples;
using Swashbuckle.AspNetCore.Filters;

namespace NgocBichKiot.Api.Controllers.Users
{
   
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        public UsersController(
            IUserService userService
          )
        {
            _userService = userService;
          
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<OrderDTO>), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UserDTOExample))]
        public async Task<IActionResult> GetAccountList()
        {
            var User = await _userService.GetUserAsync();
            return Ok(User);
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<OrderDTO>), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UserDTOExample))]
        public async Task<IActionResult> GetUserPagination([FromQuery] UserPaginationDTO paginationDTO)
        {
            try
            {
                var result = await _userService.GetPaginationAsync(paginationDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(List<OrderDTO>), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UserDTOExample))]
        public async Task<IActionResult> GetAccountById(int id)
        {
            try
            {
                var findAccountUser = await _userService.GetUserByIdAsync(id);
                return Ok(findAccountUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            
        }

        [HttpGet()]
       
        public async Task<IActionResult> GetUserProfileByUserId()
        {
            try
            {
                var findAccountUser = await _userService.GetUserProfileByUserId();
                return Ok(findAccountUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            
        }
        [HttpGet()]

        public ActionResult<CustomerInfo> GetCustomerInfoByPhone([FromQuery] string phone)
        {

            try
            {
                var customerInfo = _userService.GetCustomerInfoByPhone(phone);

                if (customerInfo == null)
                {
                    return NotFound(new { message = "Customer not found with the provided phone number." });
                }

                return Ok(customerInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO createdAccountDTO)
        {
            try
            {
                var response = await _userService.CreateAccountAsync(createdAccountDTO);
                return Created(nameof(CreateUser), response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO accountDTO)
        {
            try
            {
                var updatedUser = await _userService.UpdateUserAsync(id, accountDTO);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserProfile(int id, [FromBody] UpdateUserProfileDTO accountDTO)
        {
            try
            {
                var updatedUser = await _userService.UpdateUserProfileAsync(id, accountDTO);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            
        }
        [HttpPut("{accountId}/{isDeleted}")]
        public async Task<IActionResult> DeleteOrEnable(int accountId, int isDeleted)
        {
            try
            {
                await _userService.DeleteOrEnable(accountId, isDeleted > 0);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            
        }
    }
}
