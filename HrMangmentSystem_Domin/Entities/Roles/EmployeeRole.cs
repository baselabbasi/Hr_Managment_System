using HrMangmentSystem_Domain.Entities.Employees;

namespace HrMangmentSystem_Domain.Entities.Roles
{
    public class EmployeeRole
    {
        public string EmployeeId { get; set; } = null!;
        public Employee Employee { get; set; } = null!;


        public string RoleId { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}
