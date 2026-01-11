using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Records
{
    public class LogForPriceChangeDTO
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
       
        public string? Type { get; set; }
        public string? Name { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }
        public decimal? OldOriginalPrice { get; set; }

        public decimal? NewOriginalPrice { get; set; }

        public decimal? OldSellingPrice { get; set; }

        public decimal? NewSellingPrice { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
