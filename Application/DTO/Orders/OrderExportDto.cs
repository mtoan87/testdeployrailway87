using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Orders
{
    public class OrderExportDto
    {
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public int Unit { get; set; }
        //public string SourceOfProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime? OrderDate { get; set; }
    }
}
