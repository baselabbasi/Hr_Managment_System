
using HrMangmentSystem_Domain.Common;
using HrMangmentSystem_Domain.Enum.Request;

namespace HrMangmentSystem_Domain.Entities.Requests
{
    public class LeaveRequest : SoftDeletable<int>
    {
        public LeaveType LeaveType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int TotalDays { get; set; }

        public string? Reason { get; set; }

        public int GenericRequestId { get; set; }
        public GenericRequest GenericRequest { get; set; } = null!;
    }
}
