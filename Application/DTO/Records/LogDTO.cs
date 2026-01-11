using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Records
{
    public class LogDTO
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }      
        public int? Quantity { get; set; }
        public string? Type { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
