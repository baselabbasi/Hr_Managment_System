namespace HrMangmentSystem_Application.Employees.DTOs
{
    public class CreateEmployeeDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public int DepartmentId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Position { get; set; } = null!;

        public DateTime EmploymentStartDate { get; set; }
        public string Address { get; set; } = null!;

    }
}
