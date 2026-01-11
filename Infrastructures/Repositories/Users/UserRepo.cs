using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Model;

using System.Text;
using System.Threading.Tasks;
using Application.IRepositories.Users;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Application.Commons;
using Application.DTO.Users;
using Application.DTO.OrderDetails;
using Application.DTO.Images;
using Microsoft.Extensions.Logging;
using Application.DTO.Addresses;

namespace Infrastructures.Repositories.Users
{
    public class UserRepo : GenericRepository<User>, IUserRepo
    {
        private readonly HypeCatDbContext _context;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public UserRepo(
            HypeCatDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
            ): 
            base (context, timeService, claimsService)
        {
            _timeService = timeService;
            _context = context;
            _claimsService = claimsService;
        }


        public async Task<Pagination<UserDTO>> GetPaginationAsync(UserPaginationDTO paginationDTO)
        {
            // Truy vấn người dùng có tài khoản từ bảng Users
            var usersQuery = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Images)
                .AsQueryable();

            // Áp dụng các bộ lọc
            if (paginationDTO.IsDeleted.HasValue)
            {
                usersQuery = usersQuery.Where(u => u.IsDeleted == paginationDTO.IsDeleted);
            }

            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                usersQuery = usersQuery.Where(u =>
                    u.Name.Contains(paginationDTO.SearchTerm) ||
                    u.Email.Contains(paginationDTO.SearchTerm) ||
                    u.Phone.Contains(paginationDTO.SearchTerm));
            }

            if (!string.IsNullOrEmpty(paginationDTO.Status))
            {
                usersQuery = usersQuery.Where(u => u.Status == paginationDTO.Status);
            }

            if (paginationDTO.RoleId.HasValue)
            {
                usersQuery = usersQuery.Where(u => u.RoleId == paginationDTO.RoleId);
            }

            // Áp dụng sắp xếp
            if (!string.IsNullOrEmpty(paginationDTO.SortBy))
            {
                usersQuery = paginationDTO.SortBy.ToLower() switch
                {
                    "name" => paginationDTO.IsDescending ? usersQuery.OrderByDescending(u => u.Name) : usersQuery.OrderBy(u => u.Name),
                    "email" => paginationDTO.IsDescending ? usersQuery.OrderByDescending(u => u.Email) : usersQuery.OrderBy(u => u.Email),
                    "phone" => paginationDTO.IsDescending ? usersQuery.OrderByDescending(u => u.Phone) : usersQuery.OrderBy(u => u.Phone),
                    "createdate" => paginationDTO.IsDescending ? usersQuery.OrderByDescending(u => u.CreateDate) : usersQuery.OrderBy(u => u.CreateDate),
                    "isdeleted" => paginationDTO.IsDescending ? usersQuery.OrderByDescending(u => u.IsDeleted) : usersQuery.OrderBy(u => u.IsDeleted),
                    _ => paginationDTO.IsDescending ? usersQuery.OrderByDescending(u => u.CreateDate) : usersQuery.OrderBy(u => u.CreateDate)
                };
            }
            else
            {
                usersQuery = paginationDTO.IsDescending ? usersQuery.OrderByDescending(u => u.CreateDate) : usersQuery.OrderBy(u => u.CreateDate);
            }

            // Dựng danh sách người dùng có tài khoản
            var userResults = await usersQuery.Select(u => new UserDTO
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Password = u.Password,
                Phone = u.Phone,
                Status = u.Status,
                Role = u.Role != null ? new RoleDTO
                {
                    Id = u.Role.Id,
                    RoleName = u.Role.RoleName
                } : null,
                Images = u.Images.Select(img => new ImageDTO
                {
                    Id = img.Id,
                    UrlPath = img.UrlPath
                }).ToList()
            }).ToListAsync();

            // Tổng số người dùng
            var totalCount = userResults.Count;

            // Phân trang
            var items = userResults
                .Skip(paginationDTO.PageIndex * paginationDTO.PageSize)
                .Take(paginationDTO.PageSize)
                .ToList();

            return new Pagination<UserDTO>
            {
                Items = items,
                TotalItemsCount = totalCount,
                PageSize = paginationDTO.PageSize,
                PageIndex = paginationDTO.PageIndex
            };
        }

        public async Task<User> GetUserProfileByUserId()
        {
            var currentUserId = _claimsService.GetCurrentUserId;

            var user = await _context.Users
                .Include(o => o.Images)
                .Include(o => o.Role)
                .Include(o => o.Addresses.Where(a => (bool)a.IsDefault))
                .FirstOrDefaultAsync(o => o.Id == currentUserId);
            return user;
        }

        public Task<bool> CheckEmailNameExited(string email) =>
           _context.Users.AnyAsync(u => u.Email == email);
        public Task<bool> CheckPhoneNumberExited(string phonenumber) =>
           _context.Users.AnyAsync(u => u.Phone == phonenumber);
        public async Task<User> GetUserByEmailAndPassword(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(
                record => record.Email == email && record.Password == password
            );
            if (user is null)
            {
                throw new Exception("Email & password is not correct");
            }

            return user;
        }

        public CustomerInfo? GetCustomerInfoByPhone(string phone)
        {
            // Lấy từ bảng Users và địa chỉ mặc định (IsDefault = true)
            var user = _context.Users
                .Where(u => u.Phone == phone)
                .Select(u => new CustomerInfo
                {
                    Name = u.Name,
                    Phone = u.Phone,
                    Email = u.Email,
                    Address = _context.Addresses
                                .Where(a => a.UserId == u.Id && a.IsDefault == true)
                                .Select(a => a.Province + ", " + a.Ward + ", " + a.Street)
                                .FirstOrDefault(),
                    Addresses = _context.Addresses
                                .Where(a => a.UserId == u.Id && a.IsDeleted == false)
                                .Select(a => new AddressDTO
                                {
                                    Id = a.Id,
                                    UserId = a.UserId,
                                    Province = a.Province,
                                   // District = a.District,
                                    Ward = a.Ward,
                                    Street = a.Street,
                                    IsDefault = a.IsDefault
                                }).ToList()
                })
                .FirstOrDefault();

            // Nếu tồn tại user thì trả về luôn
            if (user != null) return user;

            // Nếu không có user, fallback sang OrderDetails
            var orderDetail = _context.OrderDetails
                .Where(od => od.Phone == phone)
                .Select(od => new CustomerInfo
                {
                    Name = od.Name,
                    Phone = od.Phone,
                    Address = od.Address,
                    Email = od.Email,
                    Addresses = new List<AddressDTO>() // Không có địa chỉ từ OrderDetails
                })
                .FirstOrDefault();

            return orderDetail;
        }
    }
}
