using Application.Commons;
using Application.DTO.Addresses;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories.Addresses
{
    public interface IAddressRepo : IGenericRepository<Address>
    {
        Task<Pagination<Address>> GetPaginationAsync(AddressPaginationDTO paginationDTO);
        Task<Address?> GetAddressByAddressIdAndUserId(int addressId);
        Task<List<Address>> GetListAddressByUserIdAsync();
    }
}
