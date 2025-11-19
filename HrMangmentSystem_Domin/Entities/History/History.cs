using HrMangmentSystem_Domain.Common;
using HrMangmentSystem_Domain.Entities.Employees;
using HrMangmentSystem_Domain.Enum.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HrMangmentSystem_Domain.Entities.History
{
    public class History : BaseEntity
    {
       public string EntityType { get; set; } = null!;

       public string EntityId { get; set; }  = null!;

        public string Action { get; set; } = null!;

        public RequestStatus? OldStatus { get; set; }

        public RequestStatus? NewStatus { get; set; }

        public string? Comment { get; set; }

        public string PerformedByEmployeeId { get; set; } = null!;
        public Employee PerformedByEmployee { get; set; } = null!;


        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
    }
}
