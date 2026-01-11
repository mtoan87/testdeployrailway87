using Application.DTO.BatchDetails;
using Application.DTO.Products;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Orders
{
    public class OrderDetailss

    {
        //        public int? OrderId { get; set; }
       
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public List<BatchUsedDTO> BatchBreakdown { get; set; } = new();
        public virtual ProductOrderDTO? Product { get; set; }
        //public virtual ProductOrderDetailDTO? Product { get; set; }

        // Navigation Properties


    }
   
}
