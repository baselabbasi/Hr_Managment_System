using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HrMangmentSystem_Domain.Common
{
    public abstract class SoftDeletable
    {
        public bool IsDeleted { get; set; } 

        public DateTime DeletedAt  { get; set; } = DateTime.UtcNow;

        public string? DeletedByEmployeeId { get; set; }
    }
}
