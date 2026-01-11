using Application.DTO.BatchDetails;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Batches
{
    public class BatchDTO
    {
        public int Id { get; set; }
        public DateTime? CreateDate { get; set; }
        public virtual ICollection<BatchDetailDTO> BatchDetails { get; set; } = new List<BatchDetailDTO>();
    }
}
