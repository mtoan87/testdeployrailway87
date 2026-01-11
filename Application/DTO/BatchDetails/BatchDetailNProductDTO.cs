using Application.DTO.Orders;
using Application.DTO.SourceOfProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.BatchDetails
{
    public class BatchDetailNProductDTO
    {
       // public string? SourceOfProductNam { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ExpiredDate { get; set; }

        public int? RemainingQuantity { get; set; }

        public int? DaysUntilExpiration { get; set; }
        public bool? IsExpiredLogged { get; set; }

        //public int? Quantity { get; set; }



        //public virtual BatchDTO BatchDTO { get; set; } = null!;

       //public virtual ProductOrderDTO ProductDTO { get; set; } = null!;

        public virtual SourceDTO? SourceOfProductDTO { get; set; }
    }
}
