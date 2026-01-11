using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Products
{
    public class ProductOrderDetailDTO
    {
        public string Name { get; set; } = null!;

        public string Category { get; set; } = null!;
        public decimal? SellingPrice { get; set; }
        public int? StockQuantity { get; set; }

    }
}
