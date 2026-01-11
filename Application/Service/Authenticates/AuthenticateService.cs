using Application.Commons;
using Application.Interfaces;
using Application.Interfaces.Authenticates;
using System;
using Domain.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.Users;
using Application.Utils;
using Application.DTO.Authenticates;
using System.Data;
using Mapster;
using Domain.Enum;

namespace Application.Service.Authenticates
{
    public class AuthenticateService : IAuthenticatesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentTime _currentTime;
        private readonly AppConfiguration _configuration;
        private readonly IClaimsService _claimsService;
        private readonly ICloudinaryService _cloudinaryService;

        public AuthenticateService
            (
            IUnitOfWork unitOfWork,
            IClaimsService claimsService,
            AppConfiguration appConfiguration,
            ICurrentTime currentTime,
            ICloudinaryService cloudinaryService
            )
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            _configuration = appConfiguration;
            _claimsService = claimsService;
            _cloudinaryService = cloudinaryService;
        }
        public async Task<Token> LoginAsync(LoginDTO accountDto)
        {
            
            var user = await _unitOfWork.UserRepo.GetUserByEmailAndPassword(
                accountDto.Email!,
                accountDto.Password!
            );
            if (user == null)
            {
                throw new Exception("Invalid username or password");
            }
            if (user.IsDeleted == true)
            {
                throw new Exception("Account is deleted");
            }          
            var token = user.GenerateJsonWebToken(
                    _configuration,
                    _configuration.JWTSection.SecretKey,
                    _currentTime.GetCurrentTime()
                );
            return new Token { AccessToken = token };
        }
        public async Task<UserDTO> RegisterAsync(RegisterUserDTO registerAccountDTO)
        {
            var exist = await _unitOfWork.UserRepo.CheckEmailNameExited(registerAccountDTO.Email);
            //var phoneExist = await _unitOfWork.UserRepo.CheckPhoneNumberExited(registerAccountDTO.Phone);
            if (exist)
            {
                throw new Exception("Email is existed");
            }
            var account = registerAccountDTO.Adapt<User>();
            account.RoleId = (int)Roles.Customer;
            account.Status = UserStatus.Active.ToString();
            await _unitOfWork.UserRepo.AddAsync(account);
            await _unitOfWork.SaveChangeAsync();

            // Upload ảnh nếu có
            if (registerAccountDTO.UserImages != null && registerAccountDTO.UserImages.Any())
            {
                var images = new List<Image>();
                foreach (var image in registerAccountDTO.UserImages)
                {
                    var (publicId, url) = await _cloudinaryService.UploadFileAsync(image, "users");

                    var userImage = new Image
                    {
                        UrlPath = url,
                        UserId = account.Id
                    };

                    images.Add(userImage);
                }
                await _unitOfWork.ImageRepo.AddRangeAsync(images);
                await _unitOfWork.SaveChangeAsync();
            }

            // Lấy lại user với ảnh đã upload
            account = await _unitOfWork.UserRepo.GetById(account.Id, includeProperties:"Images,Role");
            return account.Adapt<UserDTO>();
        }
    }
}
