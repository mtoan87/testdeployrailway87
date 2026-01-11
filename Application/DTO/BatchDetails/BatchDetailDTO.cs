using Application.DTO.Batches;
using Application.DTO.Orders;
using Application.DTO.Products;
using Application.DTO.SourceOfProducts;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.BatchDetails
{
    public class BatchDetailDTO
    {
        public int Id { get; set; }

        public int BatchId { get; set; }

        public int ProductId { get; set; }

      

        public decimal? SellingPrice { get; set; }

        public decimal? ImportCosts { get; set; }

        public int? RemainingQuantity { get; set; }

     
        public int? Quantity { get; set; }

        public int? BatchDetailParentId { get; set; }


        public virtual ProductOrderDTO ProductDTO { get; set; } = null!;

       
    }
}
