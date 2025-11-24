using System.ComponentModel.DataAnnotations;

namespace HrMangmentSystem_Application.DTOs.Employee
{
    public class CreateEmployeeDto
    {
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;

        [EmailAddress]
        public string Email { get; set; } = null!;

        [Phone]
        public string PhoneNumber { get; set; } = null!;

        public int DepartmentId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Position { get; set; } = null!;

        public DateTime EmploymentStartDate { get; set; }
        public string Address { get; set; } = null!;

        [Required]
        public Guid TenantId { get; set; }
    }
}
