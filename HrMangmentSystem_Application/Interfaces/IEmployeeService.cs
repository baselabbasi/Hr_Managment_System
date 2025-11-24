using HrMangmentSystem_Application.Common;
using HrMangmentSystem_Application.DTOs;

namespace HrMangmentSystem_Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<ApiResponse<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto);
        Task<ApiResponse<EmployeeDto?>> GetEmployeeByIdAsync(Guid employeeId);
        Task<ApiResponse<List<EmployeeDto>>> GetAllEmployeesAsync();
        Task<ApiResponse<bool>> UpdateEmployeeAsync(UpdateEmployeeDto updateEmployeeDto);
        Task<ApiResponse<bool>> DeleteEmployeeAsync(Guid employeeId, Guid? deletedByEmployeeId);
    }
}
