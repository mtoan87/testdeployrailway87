using Application.DTO.OrderDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Orders
{
    public class UpdateOrderDTO
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public List<UpdateOrderDetailDTO> OrderDetails { get; set; } = new List<UpdateOrderDetailDTO>();

        public List<int> DeletedProductIds { get; set; }
    }
}
