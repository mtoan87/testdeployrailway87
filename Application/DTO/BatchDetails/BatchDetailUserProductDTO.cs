using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.BatchDetails
{
    public class BatchDetailUserProductDTO
    {
        public int Id { get; set; }
        public decimal? SellingPrice { get; set; }
        public int? RemainingQuantity { get; set; }
    }
}
