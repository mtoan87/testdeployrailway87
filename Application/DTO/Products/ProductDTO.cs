using Application.DTO.Categories;
using Application.DTO.Images;
using Application.DTO.Records;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Products
{
    public class ProductDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual CategoryDTO? Category { get; set; }

        public decimal? OriginalPrice { get; set; }

        public decimal? SellingPrice { get; set; }

        public string? SourceOfProducts { get; set; }

        public decimal? ImportCosts { get; set; }

        public int? StockQuantity { get; set; }

        public string? Status { get; set; }
        public virtual ICollection<ImageDTO> Images { get; set; } = new List<ImageDTO>();
        public virtual ICollection<ProductLogListDTO> Logs { get; set; } = new List<ProductLogListDTO>();


    }
}
