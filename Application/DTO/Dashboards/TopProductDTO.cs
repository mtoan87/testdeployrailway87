using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Dashboards
{
    public class TopProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public decimal SellingPrice { get; set; }
        public int TotalQuantitySold { get; set; }

        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
