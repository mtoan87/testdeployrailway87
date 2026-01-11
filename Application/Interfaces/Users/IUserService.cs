using Application.Commons;
using Application.DTO.OrderDetails;
using Application.DTO.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Users
{
    public interface IUserService
    {

        Task<UserDTO> UpdateUserProfileAsync(int id, UpdateUserProfileDTO accountDTO);
        Task<IEnumerable<UserDTO>> GetUserAsync();
        Task<Pagination<UserDTO>> GetPaginationAsync(UserPaginationDTO paginationDTO);
        Task<UserDTO> GetUserProfileByUserId();
        Task<UserDTO> GetUserByIdAsync(int id);
        Task DeleteOrEnable(int accountId, bool isDeleted);
        Task<UserDTO> UpdateUserAsync(int id, UpdateUserDTO accountDTO);
        Task<UserDTO> CreateAccountAsync(CreateUserDTO createdAccountDto);
        CustomerInfo? GetCustomerInfoByPhone(string phone);
    }
}
