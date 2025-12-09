using HrMangmentSystem_Domain.Common;
using HrMangmentSystem_Domain.Enum.Request;

namespace HrMangmentSystem_Domain.Entities.Employees
{

    public class EmployeeLeaveBalance : SoftDeletable<int>
    {
        public Guid EmployeeId { get; set; }

        public LeaveType LeaveType { get; set; }

    
        public decimal BalanceDays { get; set; }

        public DateTime LastUpdatedAt { get; set; }
    }
}
