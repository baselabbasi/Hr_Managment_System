namespace HrMangmentSystem_Application.Employees.DTOs
{
    public class UpdateEmployeeDto
    {
        public Guid Id { get; set; }

        public string? PhoneNumber { get; set; }
        public int? DepartmentId { get; set; }
        public string? Position { get; set; }
        public string? Address { get; set; }
        
    }
}
