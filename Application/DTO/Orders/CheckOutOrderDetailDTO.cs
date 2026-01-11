using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Orders
{
    public class CheckOutOrderDetailDTO
    {

        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        // Navigation Properties
       
        public ProductOrderDTO? Product { get; set; }
    }
}
