using Application.DTO.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.OrderDetails
{
    public class OrderDetailWithUserInfo
    {
       
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
                   
        public ProductOrderDTO? Product { get; set; }
    }
}
