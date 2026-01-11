using Application.Commons;
using Application.DTO.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Addresses
{
    public interface IAddressService
    {
        Task<AddressDTO> AddAddress(CreateAddressDTO addAddress);
        Task<Pagination<AddressDTO>> GetPaginationAsync(AddressPaginationDTO paginationDTO);
        Task<List<AddressDTO>> GetAddressByUserId();

        Task DefaultOrNot(int addressId, bool isDefault, int userId);
        Task<AddressDTO?> GetAddressById(int id);
        Task<AddressDTO> UpdateAddress(int id, UpdateAddressDTO updateAddress);
        Task<List<AddressDTO>> GetAllAddresses();
        Task DeleteOrEnable(int categoryId, bool isDeleted);
    }
}
