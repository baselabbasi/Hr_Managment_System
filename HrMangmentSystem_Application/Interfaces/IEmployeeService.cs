using HrMangmentSystem_Application.Common.PagedRequest;
using HrMangmentSystem_Application.Common.Responses;
using HrMangmentSystem_Application.DTOs.Employee;

namespace HrMangmentSystem_Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<ApiResponse<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto);
        Task<ApiResponse<EmployeeDto?>> GetEmployeeByIdAsync(Guid employeeId);
        Task<ApiResponse<List<EmployeeDto>>> GetAllEmployeesAsync();
        Task<ApiResponse<EmployeeDto>> UpdateEmployeeAsync(UpdateEmployeeDto updateEmployeeDto);
        Task<ApiResponse<bool>> DeleteEmployeeAsync(Guid employeeId, Guid deletedByEmployeeId);

        Task<ApiResponse<PagedResult<EmployeeDto>>> GetEmployeesPagedAsync(PagedRequest request);
    }
}
