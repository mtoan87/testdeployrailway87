using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.BatchDetails
{
    public class CreateBatchDetailDTO
    {
        public int ProductId { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? ImportCosts { get; set; }
        //public string? SourceOfProductName { get; set; }
        public int Quantity { get; set; }
        //public int? SourceOfProductId { get; set; }
        //public DateTime? ExpiredDate { get; set; }
    }
}
