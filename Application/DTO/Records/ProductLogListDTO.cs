using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Records
{
    public class ProductLogListDTO
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int? Quantity { get; set; }
        public decimal? OldOriginalPrice { get; set; }

        public decimal? NewOriginalPrice { get; set; }

        public decimal? OldSellingPrice { get; set; }

        public decimal? NewSellingPrice { get; set; }

        public decimal? OldImportCost { get; set; }

        public decimal? NewImportCost { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? Type { get; set; }

        
    }
}
