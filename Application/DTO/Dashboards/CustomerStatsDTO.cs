using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Dashboards
{
    public class CustomerStatsDTO
    {
        public int TotalCustomer { get; set; }
        public int ImportCustomer { get; set; }
        public int ExportCustomer { get; set; }
        public int BothTypeCustomer { get; set; }
    }
}
