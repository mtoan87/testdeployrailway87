using Application.Commons;
using Application.DTO.Addresses;
using Application.DTO.Categories;
using Application.Interfaces;
using Application.Interfaces.Addresses;
using Domain.Model;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Addresses
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<Pagination<AddressDTO>> GetPaginationAsync(AddressPaginationDTO paginationDTO)
        {
            try
            {
                var pagination = await _unitOfWork.AddressRepo.GetPaginationAsync(paginationDTO);

                return new Pagination<AddressDTO>
                {
                    Items = pagination.Items.Adapt<List<AddressDTO>>(),
                    TotalItemsCount = pagination.TotalItemsCount,
                    PageSize = pagination.PageSize,
                    PageIndex = pagination.PageIndex
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting pagination: {ex.Message}", ex);
            }
        }
        public async Task<AddressDTO> AddAddress(CreateAddressDTO addAddress)
        {
            var user = await _unitOfWork.UserRepo.GetByIdAsync(addAddress.UserId);
            if (user is null)
            {
                throw new Exception("User does not exist.");
            }

            // Nếu địa chỉ mới là default thì các địa chỉ cũ phải unset default
            if (addAddress.IsDefault)
            {
                var otherAddresses = await _unitOfWork.AddressRepo
                    .GetAllAsync(a => a.UserId == addAddress.UserId);

                foreach (var addr in otherAddresses)
                {
                    addr.IsDefault = false;
                }
            }

            var address = addAddress.Adapt<Address>();
            await _unitOfWork.AddressRepo.AddAsync(address);
            await _unitOfWork.SaveChangeAsync();

            return address.Adapt<AddressDTO>();
        }
        public async Task<List<AddressDTO>> GetAddressByUserId()
        {
            var category = await _unitOfWork.AddressRepo.GetListAddressByUserIdAsync();
            if (category is null)
            {
                throw new Exception("Address is not existed");
            }

            return category.Adapt<List<AddressDTO>>();
        }
        public async Task<AddressDTO?> GetAddressById(int id)
        {
            var category = await _unitOfWork.AddressRepo.GetAsync(x => x.Id == id);
            if (category is null)
            {
                throw new Exception("Address is not existed");
            }

            return category.Adapt<AddressDTO>();
        }

        public async Task<AddressDTO> UpdateAddress(int id, UpdateAddressDTO updateAddress)
        {
            var address = await _unitOfWork.AddressRepo.GetByIdAsync(id);
            if (address is null)
            {
                throw new Exception("Address does not exist.");
            }

            // Nếu địa chỉ đang được cập nhật thành mặc định
            if (updateAddress.IsDefault)
            {
                var otherAddresses = await _unitOfWork.AddressRepo
                    .GetAllAsync(d => d.UserId == address.UserId && d.Id != id);

                foreach (var addr in otherAddresses)
                {
                    addr.IsDefault = false;
                }
            }

            updateAddress.Adapt(address);
            _unitOfWork.AddressRepo.Update(address);
            await _unitOfWork.SaveChangeAsync();

            return address.Adapt<AddressDTO>();
        }
        public async Task<List<AddressDTO>> GetAllAddresses()
        {
            var categories = await _unitOfWork.AddressRepo.GetAllAsync();
            return categories.Adapt<List<AddressDTO>>();
        }

        public async Task DeleteOrEnable(int categoryId, bool isDeleted)
        {
            var address = await _unitOfWork.AddressRepo.GetAsync(d => d.Id == categoryId);
            if (address is null)
            {
                throw new Exception("Address does not exist.");
            }
            address.IsDeleted = isDeleted;
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task DefaultOrNot(int addressId, bool isDefault, int userId)
        {
            var address = await _unitOfWork.AddressRepo.GetAsync(d => d.Id == addressId && d.UserId == userId);
            if (address is null)
            {
                throw new Exception("Address does not exist.");
            }

            if (isDefault)
            {
                // Unset IsDefault for all other addresses of the user
                var userAddresses = await _unitOfWork.AddressRepo
                    .GetAllAsync(d => d.UserId == userId && d.Id != addressId);

                foreach (var addr in userAddresses)
                {
                    addr.IsDefault = false;
                }
            }

            address.IsDefault = isDefault;
            await _unitOfWork.SaveChangeAsync();
        }

    }
}
