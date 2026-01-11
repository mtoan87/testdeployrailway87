using Application.DTO.BatchDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Batches
{
    public class BatchWDetailDTO
    {
        public int Id { get; set; }
        public DateTime? CreateDate { get; set; }
        public virtual ICollection<BatchDetailGroupDTO> BatchDetailDTOs { get; set; } = new List<BatchDetailGroupDTO>();
    }
}
