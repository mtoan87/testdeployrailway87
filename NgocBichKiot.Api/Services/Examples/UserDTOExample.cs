using Application.DTO.Images;
using Application.DTO.Users;
using Swashbuckle.AspNetCore.Filters;

namespace NgocBichKiot.Api.Services.Examples
{
    public class UserDTOExample : IExamplesProvider<UserDTO>
    {
        public UserDTO GetExamples()
        {
            return new UserDTO
            {
                Id = 8,
                Name = "admin",
                Email = "admin@gmail.com",
                Password = "123",
                Phone = "012345678",
                Address = "VN",
                Status = "Active",
                Role = new RoleDTO
                {
                    Id = 1,
                    RoleName = "Admin"
                },
                Images = new List<ImageDTO>
                {
                    new ImageDTO
                    {
                        Id = 13,
                        UrlPath = "https://res.cloudinary.com/ducxotvyo/image/upload/v1744641083/ngocbichkiot/users/srunmxfcwlwkxgd9ly2l.jpg"
                    }
                }
            };
        }
    }
}
