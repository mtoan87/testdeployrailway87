using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Products
{
    public class UpdateProductQuantity
    {
        //public int Id { get; set; }

        public string Name { get; set; } = null!;

        //public string Category { get; set; } = null!;

        public decimal? OriginalPrice { get; set; }

        public decimal? SellingPrice { get; set; }

        //public string? SourceOfProducts { get; set; }

        public decimal? ImportCosts { get; set; }

        public int? StockQuantity { get; set; }

        public string? Unit { get; set; }

        public string? Status { get; set; }
    }
}
