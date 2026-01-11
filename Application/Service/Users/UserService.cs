using Application.Commons;
using Application.DTO.OrderDetails;
using Application.DTO.Users;
using Application.Interfaces;
using Application.Interfaces.Users;
using Application.IRepositories.Users;
using CloudinaryDotNet.Actions;
using Domain.Enum;
using Domain.Model;
using Mapster;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Users
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
          
        }
        public async Task<IEnumerable<UserDTO>> GetUserAsync()
        {
            var accounts = await _unitOfWork.UserRepo.GetAllAsync(null,includeProperties: "Role,Images");
            return accounts.Adapt<IEnumerable<UserDTO>>();
        }

        public async Task<UserDTO> GetUserProfileByUserId()
        {
            var accounts = await _unitOfWork.UserRepo.GetUserProfileByUserId();
            if(accounts == null)
            {
                throw new Exception("Account is not existed");
            }
            return accounts.Adapt<UserDTO>();
        }
        public async Task<Pagination<UserDTO>> GetPaginationAsync(UserPaginationDTO paginationDTO)
        {
            try
            {
                var pagination = await _unitOfWork.UserRepo.GetPaginationAsync(paginationDTO);

                return new Pagination<UserDTO>
                {
                    Items = pagination.Items.Adapt<List<UserDTO>>(),
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
        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            var exist = await _unitOfWork.UserRepo.GetById(id , includeProperties:"Images,Role,Addresses");
            if (exist == null)
            {
                throw new Exception("Account is not existed");
            }
            return exist.Adapt<UserDTO>();
        }
        public async Task<UserDTO> CreateAccountAsync(CreateUserDTO createdAccountDto)
        {
            try
            {
                var existed = await _unitOfWork.UserRepo.CheckPhoneNumberExited(createdAccountDto.Phone!);
                if (existed)
                {
                    throw new Exception("Phone is existed");
                }

                var account = createdAccountDto.Adapt<User>();
                account.Status = UserStatus.Active.ToString();

                await _unitOfWork.UserRepo.AddAsync(account);
                await _unitOfWork.SaveChangeAsync(); // 🔑 để có được account.Id

                // 👉 Tạo địa chỉ mặc định cho user
                var address = new Address
                {
                    UserId = account.Id,
                    RecipientName = createdAccountDto.Name,
                    Phone = createdAccountDto.Phone,
                    Province = createdAccountDto.Province,
                    Ward = createdAccountDto.Ward,
                    Street = createdAccountDto.Street,
                    IsDefault = true,
                    
                };
                await _unitOfWork.AddressRepo.AddAsync(address);
                await _unitOfWork.SaveChangeAsync();

                // 👉 Lưu hình ảnh (nếu có)
                if (createdAccountDto.UserImages != null && createdAccountDto.UserImages.Any())
                {
                    foreach (var url in createdAccountDto.UserImages)
                    {
                        var image = new Image
                        {
                            UrlPath = url,
                            UserId = account.Id,
                           
                        };
                        await _unitOfWork.ImageRepo.AddAsync(image);
                    }
                }

                await _unitOfWork.SaveChangeAsync();

                account = await _unitOfWork.UserRepo.GetById(account.Id, includeProperties: "Role,Images,Addresses"); // 👈 include Addresses nếu cần
                return account.Adapt<UserDTO>();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while create user: {ex.Message}", ex);
            }
        }

        //public async Task<UserDTO> CreateAccountAsync(CreateUserDTO createdAccountDto)
        //{
        //    try
        //    {
        //        var exist = await _unitOfWork.UserRepo.CheckEmailNameExited(createdAccountDto.Email!);
        //        var existed = await _unitOfWork.UserRepo.CheckPhoneNumberExited(createdAccountDto.Phone!);

        //        if (exist)
        //        {
        //            throw new Exception("Email is existed");
        //        }
        //        if (existed)
        //        {
        //            throw new Exception("Phone is existed");
        //        }

        //        var account = createdAccountDto.Adapt<User>();
        //        account.Status = UserStatus.Active.ToString();
        //        await _unitOfWork.UserRepo.AddAsync(account);
        //        await _unitOfWork.SaveChangeAsync();

        //        // Upload multiple images
        //        if (createdAccountDto.UserImages != null && createdAccountDto.UserImages.Any())
        //        {
        //            var images = new List<Image>();
        //            foreach (var image in createdAccountDto.UserImages)
        //            {
        //                var (publicId, url) = await _cloudinaryService.UploadFileAsync(image, "users");

        //                var userImage = new Image
        //                {
        //                    UrlPath = url,
        //                    UserId = account.Id
        //                };

        //                images.Add(userImage);
        //            }
        //            await _unitOfWork.ImageRepo.AddRangeAsync(images);
        //            await _unitOfWork.SaveChangeAsync();
        //        }
        //        account = await _unitOfWork.UserRepo.GetById(account.Id, includeProperties: "Images,Role");
        //        return account.Adapt<UserDTO>();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error creating user");
        //        throw;
        //    }
        //}

        public async Task<UserDTO> UpdateUserAsync(int id, UpdateUserDTO accountDTO)
        {
            try
            {
                var existingUser = await _unitOfWork.UserRepo.GetByIdAsync(id);
                if (existingUser == null)
                {
                    throw new Exception("Account is not existed");
                }
                if (existingUser.IsDeleted == true)
                {
                    throw new Exception("Account is deleted in system");
                }

                // Cập nhật thông tin người dùng
                _unitOfWork.UserRepo.Update(accountDTO.Adapt(existingUser));
                await _unitOfWork.SaveChangeAsync();

                // Xử lý ảnh nếu có
                if (accountDTO.UserImages != null)
                {
                    var existingImages = await _unitOfWork.ImageRepo.GetAllAsync(x => x.UserId == id);
                    var existingUrls = existingImages.Select(i => i.UrlPath).ToList();
                    var newUrls = accountDTO.UserImages;

                    // 1. Xóa ảnh cũ không còn sử dụng (xóa cứng)
                    var toDelete = existingImages.Where(i => !newUrls.Contains(i.UrlPath)).ToList();
                    foreach (var img in toDelete)
                    {
                        await _unitOfWork.ImageRepo.DeleteAsync(img);
                    }

                    // 2. Thêm ảnh mới
                    var toAdd = newUrls.Where(url => !existingUrls.Contains(url)).ToList();
                    foreach (var url in toAdd)
                    {
                        var image = new Image
                        {
                            UrlPath = url,
                            UserId = id,
                            
                        };
                        await _unitOfWork.ImageRepo.AddAsync(image);
                    }

                    await _unitOfWork.SaveChangeAsync();
                }

                existingUser = await _unitOfWork.UserRepo.GetById(id, includeProperties: "Role");
                return existingUser.Adapt<UserDTO>();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while update user: {ex.Message}", ex);
                
            }
        }

        public async Task<UserDTO> UpdateUserProfileAsync(int id, UpdateUserProfileDTO accountDTO)
        {
            try
            {
                var existingUser = await _unitOfWork.UserRepo.GetByIdAsync(id);
                if (existingUser == null)
                {
                    throw new Exception("Account is not existed");
                }
                if (existingUser.IsDeleted == true)
                {
                    throw new Exception("Account is deleted in system");
                }

                // Cập nhật thông tin người dùng
                _unitOfWork.UserRepo.Update(accountDTO.Adapt(existingUser));
                await _unitOfWork.SaveChangeAsync();

                // Xử lý ảnh nếu có
                if (accountDTO.UserImages != null)
                {
                    var existingImages = await _unitOfWork.ImageRepo.GetAllAsync(x => x.UserId == id);
                    var existingUrls = existingImages.Select(i => i.UrlPath).ToList();
                    var newUrls = accountDTO.UserImages;

                    // 1. Xóa ảnh cũ không còn sử dụng (xóa cứng)
                    var toDelete = existingImages.Where(i => !newUrls.Contains(i.UrlPath)).ToList();
                    foreach (var img in toDelete)
                    {
                        await _unitOfWork.ImageRepo.DeleteAsync(img);
                    }

                    // 2. Thêm ảnh mới
                    var toAdd = newUrls.Where(url => !existingUrls.Contains(url)).ToList();
                    foreach (var url in toAdd)
                    {
                        var image = new Image
                        {
                            UrlPath = url,
                            UserId = id,

                        };
                        await _unitOfWork.ImageRepo.AddAsync(image);
                    }

                    await _unitOfWork.SaveChangeAsync();
                }

                existingUser = await _unitOfWork.UserRepo.GetById(id, includeProperties: "Role");
                return existingUser.Adapt<UserDTO>();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while update user: {ex.Message}", ex);

            }
        }


        //public async Task<UserDTO> UpdateUserAsync(int id, UpdateUserDTO accountDTO)
        //{
        //    try
        //    {
        //        var existingUser = await _unitOfWork.UserRepo.GetByIdAsync(id);
        //        if (existingUser == null)
        //        {
        //            throw new Exception("Account is not existed");
        //        }
        //        if (existingUser.IsDeleted == true)
        //        {
        //            throw new Exception("Account is deleted in system");
        //        }

        //        // Lấy danh sách ảnh hiện tại của user
        //        var currentImages = await _unitOfWork.ImageRepo.GetAllAsync(x => x.UserId == id);

        //        // Xóa các ảnh được chọn để xóa (nếu có)
        //        if (accountDTO.ImageIdsToDelete != null && accountDTO.ImageIdsToDelete.Any())
        //        {
        //            foreach (var imageId in accountDTO.ImageIdsToDelete)
        //            {
        //                // Kiểm tra xem ảnh có thuộc về user không
        //                var imageToDelete = currentImages.FirstOrDefault(x => x.Id == imageId && x.UserId == id);
        //                if (imageToDelete != null)
        //                {
        //                    // Xóa ảnh từ Cloudinary
        //                    if (!string.IsNullOrEmpty(imageToDelete.UrlPath))
        //                    {
        //                        var publicId = _cloudinaryService.GetPublicIdFromUrl(imageToDelete.UrlPath);
        //                        await _cloudinaryService.DeleteFileAsync(publicId);
        //                    }

        //                    // Xóa ảnh từ database
        //                    await _unitOfWork.ImageRepo.DeleteAsync(imageToDelete);
        //                }
        //            }
        //        }

        //        // Upload ảnh mới nếu có
        //        if (accountDTO.UserImages != null && accountDTO.UserImages.Any())
        //        {
        //            var images = new List<Image>();
        //            foreach (var image in accountDTO.UserImages)
        //            {
        //                var (publicId, url) = await _cloudinaryService.UploadFileAsync(image, "users");

        //                var userImage = new Image
        //                {
        //                    UrlPath = url,
        //                    UserId = id
        //                };

        //                images.Add(userImage);
        //            }
        //            await _unitOfWork.ImageRepo.AddRangeAsync(images);
        //        }

        //        _unitOfWork.UserRepo.Update(accountDTO.Adapt(existingUser));
        //        await _unitOfWork.SaveChangeAsync();

        //        // Lấy lại user với ảnh đã cập nhật
        //        existingUser = await _unitOfWork.UserRepo.GetById(id, includeProperties: "Images");
        //        return existingUser.Adapt<UserDTO>();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error updating user");
        //        throw;
        //    }
        //}


        public async Task DeleteOrEnable(int accountId, bool isDeleted)
        {
            var account = await _unitOfWork.UserRepo.GetAsync(d => d.Id == accountId);
            if (account is null)
            {
                throw new Exception("Account is not existed");
            }
            account.Status = isDeleted
            ? UserStatus.InActive.ToString()
            : UserStatus.Active.ToString();
            account.IsDeleted = isDeleted;
            await _unitOfWork.SaveChangeAsync();
        }

        public  CustomerInfo? GetCustomerInfoByPhone(string phone)
        {
            return  _unitOfWork.UserRepo.GetCustomerInfoByPhone(phone);
        }
    }
}
