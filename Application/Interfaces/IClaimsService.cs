using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IClaimsService
    {
        public int? GetCurrentUserRole { get; }
        public string? GetCurrentUserName { get; }
        public int GetCurrentUserId { get; }
    }
}
