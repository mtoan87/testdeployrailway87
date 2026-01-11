using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Dashboards
{
    public class InventoryExportRequest
    {
        public int? ProductId { get; set; } // null = tất cả sản phẩm
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
