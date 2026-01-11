using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Users
{
    public class UpdateUserProfileDTO
    {
        public string? Name { get; set; }

        public string? Email { get; set; }

        //public string? Password { get; set; }

        public string? Phone { get; set; }

       // public string? Address { get; set; }
        //public List<int>? ImageIdsToDelete { get; set; }
        public List<string>? UserImages { get; set; }
    }
}
