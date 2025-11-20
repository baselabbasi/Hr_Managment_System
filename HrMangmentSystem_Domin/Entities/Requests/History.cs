using HrMangmentSystem_Domain.Common;
using HrMangmentSystem_Domain.Entities.Employees;
using HrMangmentSystem_Domain.Enum.Request;

namespace HrMangmentSystem_Domain.Entities.Requests
{
    public class RequestHistory : BaseEntity<int>
    {
        public int GenericRequestId { get; set; }
        public GenericRequest GenericRequest { get; set; } = null!;

        public string Action { get; set; } = null!;

        public RequestStatus? OldStatus { get; set; }

        public RequestStatus? NewStatus { get; set; }

        public string? Comment { get; set; }

        public string PerformedByEmployeeId { get; set; } = null!;
        public Employee PerformedByEmployee { get; set; } = null!;


        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;

        //edit history
    }
}
