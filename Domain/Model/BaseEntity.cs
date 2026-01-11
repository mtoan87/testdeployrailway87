using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public abstract class BaseEntity 
    {
        public int Id { get; set; }

        public DateTime? CreateDate { get; set; }

        public string? CreateBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }
    }
}
