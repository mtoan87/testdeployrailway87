using Application.DTO.Authenticates;
using Application.DTO.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Authenticates
{
    public interface IAuthenticatesService
    {
        Task<Token> LoginAsync(LoginDTO accountDto);
        Task<UserDTO> RegisterAsync(RegisterUserDTO registerAccountDTO);
    }
}
