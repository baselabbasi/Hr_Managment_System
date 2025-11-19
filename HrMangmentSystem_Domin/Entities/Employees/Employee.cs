using HrMangmentSystem_Domain.Common;
using HrMangmentSystem_Domain.Entities.Roles;
using HrMangmentSystem_Domain.Enum.Employee;

namespace HrMangmentSystem_Domain.Entities.Employees
{
    public class Employee : BaseEntity
    {
        public string? ManagerId { get; set; }
        public Employee? Manager { get; set; }

        public string DepartmentId { get; set; } = null!;

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }

        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;


        public Gender Gender { get; set; }

        public string Password { get; set; } = null!;
        public string Position { get; set; } = null!;
        public string Address { get; set; } = null!;

        public DateTime EmploymentStartDate { get; set; }
        public DateTime? EmploymentEndDate { get; set; }


        public EmployeeStatus EmploymentStatusType { get; set; }

        public Department Department { get; set; } =  null!;
      
        public ICollection<Employee> Subordinates { get; set; } = new List<Employee>(); //employees managed by this employee

        public ICollection<EmployeeRole> EmployeeRoles { get; set; } = new List<EmployeeRole>();

    }
}
