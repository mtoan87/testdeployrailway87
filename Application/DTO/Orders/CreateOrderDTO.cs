using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Orders
{
    public class CreateOrderDTO
    {
        //public int UserId { get; set; }
        public string? Name { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }
        
        public string? Email { get; set; }


        public List<CreateOrderDetailDTO> OrderDetails { get; set; }
    }
}
