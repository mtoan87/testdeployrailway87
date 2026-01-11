using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Carts
{
    public class CartItemDisplayDTO
    {
       public int CartId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal SellingPrice { get; set; }
        public string? ImageUrl { get; set; }
        public int BatchDetailId { get; set; }
        public int Quantity { get; set; }
        public int RemainingQuantity { get; set; }
       
        public decimal TotalPrice { get; set; }
    }
}
