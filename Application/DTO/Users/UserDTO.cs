using Application.DTO.Addresses;
using Application.DTO.Images;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Users
{
    public class UserDTO
    {
        public int? Id { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }
        public string? Status { get; set; }

        public virtual RoleDTO? Role { get; set; }
        public virtual ICollection<ImageDTO> Images { get; set; } = new List<ImageDTO>();

        public virtual ICollection<AddressDTO> Addresses { get; set; } = new List<AddressDTO>();


    }
}
