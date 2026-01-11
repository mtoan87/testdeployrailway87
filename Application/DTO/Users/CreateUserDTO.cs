using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Users
{
    public class CreateUserDTO
    {
        //public int Id { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Phone { get; set; }

        public string? Province { get; set; }

        public string? Ward { get; set; }

        public string? Street { get; set; }
        public List<string> UserImages { get; set; } = new List<string>();
        public int? RoleId { get; set; }

        //public string? Status { get; set; }
    }
}
