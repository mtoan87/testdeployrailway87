using Application.Commons;
using Application.DTO.OrderDetails;
using Application.DTO.Users;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories.Users
{
    public interface IUserRepo : IGenericRepository<User>
    {
        Task<Pagination<UserDTO>> GetPaginationAsync(UserPaginationDTO paginationDTO);
        Task<bool> CheckEmailNameExited(string email);
        Task<bool> CheckPhoneNumberExited(string phonenumber);
        Task<User> GetUserByEmailAndPassword(string email, string password);
        Task<User> GetUserProfileByUserId();
        CustomerInfo? GetCustomerInfoByPhone(string phone);
    }
}
