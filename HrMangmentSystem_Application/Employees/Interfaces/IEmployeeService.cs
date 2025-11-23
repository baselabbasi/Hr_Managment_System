using HrMangmentSystem_Application.Employees.DTOs;

namespace HrMangmentSystem_Application.Employees.Interfaces
{
    public interface IEmployeeService
    {
        Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto);
        Task<EmployeeDto?> GetEmployeeByIdAsync(Guid employeeId);
        Task<List<EmployeeDto>> GetAllEmployeesAsync();
        Task<bool> UpdateEmployeeAsync(UpdateEmployeeDto updateEmployeeDto);
        Task<bool> DeleteEmployeeAsync(Guid employeeId, Guid? deletedByEmployeeId);
    }
}
