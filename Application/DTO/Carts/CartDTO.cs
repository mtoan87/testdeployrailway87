using Application.DTO.BatchDetails;
using Application.DTO.Products;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Carts
{
    public class CartDTO
    { 
        public int Id { get; set; }
        public int UserId { get; set; }

        //public int BatchDetailId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal? TotalPrice { get; set; }

        public virtual ProductCartDTO Product { get; set; } = null!;

    }
}
