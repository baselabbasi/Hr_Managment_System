using HrMangmentSystem_Domain.Enum.Request;

namespace HrMangmentSystem_Application.DTOs.Requests.EmployeeData
{
    public class EmployeeLeaveBalanceDto
    {
        public LeaveType LeaveType { get; set; }
        public decimal BalanceDays { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
