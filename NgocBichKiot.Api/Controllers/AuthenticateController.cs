using Application.DTO.Users;
using Application.Interfaces.Authenticates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NgocBichKiot.Api.Controllers
{
    public class AuthenticateController : BaseController
    {
        private readonly IAuthenticatesService _authenticatesService;
        public AuthenticateController(IAuthenticatesService authenticatesService)
        {
            _authenticatesService = authenticatesService;
        }

        [HttpPost]
       
        public async Task<IActionResult> LoginAsync(LoginDTO loginObject)
        {
          
            try
            {
                var result = await _authenticatesService.LoginAsync(loginObject);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
       
        public async Task<IActionResult> RegisterAsync([FromBody]RegisterUserDTO Object)
        {
            
            try
            {
                var result = await _authenticatesService.RegisterAsync(Object);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
