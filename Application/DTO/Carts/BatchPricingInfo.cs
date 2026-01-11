using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Carts
{
    public class BatchPricingInfo
    {
        public int BatchDetailId { get; set; }
        public decimal SellingPrice { get; set; }
        public int Quantity { get; set; }

        public int RemainingQuantity { get; set; }
    }
}
