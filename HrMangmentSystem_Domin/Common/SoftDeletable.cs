using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HrMangmentSystem_Domain.Common
{
    public  class  SoftDeletable<T>  : TenantEntity<T> 
    {
        public bool IsDeleted { get; set; } 

        public DateTime DeletedAt  { get; set; } 

        public Guid? DeletedByEmployeeId { get; set; }
    }
}
