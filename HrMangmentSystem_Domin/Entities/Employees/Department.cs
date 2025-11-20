using HrMangmentSystem_Domain.Common;

namespace HrMangmentSystem_Domain.Entities.Employees
{
    public class Department : SoftDeletable<int>
    {
        public string? ParentDepartmentId { get; set; } 
        public string Code { get; set; } = null!;

        public string DeptName { get; set; } = null!;

        public string? Description { get; set; }

        public string Location { get; set; } = null!;

        
        public string? DepartmentManagerId { get; set; }
        public Employee? DepartmentManager { get; set; }

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
       
    }
}
