using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Products
{
    public class UpdateProductDTO
    {
        //public int Id { get; set; }
        public string Name { get; set; } = null!;

        //public int? BoxId { get; set; }
        public string? Cover { get; set; }
        public int? PacksPerUnit { get; set; }

        public string Language { get; set; } = null!;

        public string? Description { get; set; }

        public string? Status { get; set; }


        public int CategoryId { get; set; }
        //public string Name { get; set; } = null!;

       // public string Category { get; set; } = null!;

        //public decimal OriginalPrice { get; set; }
        //public int CategoryId { get; set; }

       // public int SourceOfProductName { get; set; }
        //public decimal? SellingPrice { get; set; }

       // public string? SourceOfProducts { get; set; }
        //public string? UpdatedByName { get; set; }
        //public string? UpdatedByPhone { get; set; }
        //public string? UpdatedByAddress { get; set; }
        //public decimal? ImportCosts { get; set; }
        //public string? Unit { get; set; }             
        //public string? Status { get; set; }

        public List<string> ProductImages { get; set; } = new List<string>();
    }
}
