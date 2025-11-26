using HrMangmentSystem_Application.Common.Responses;
using HrMangmentSystem_Application.DTOs.Department;

namespace HrMangmentSystem_Application.Interfaces
{
    public interface IDepartmentService
    {
       Task<ApiResponse<DepartmentDto>> CreateDepartmentAsync(CreateDepartmentDto createDepartmentDto);

        Task<ApiResponse<DepartmentDto?>> GetDepartmentByIdAsync(int departmentId);

        Task<ApiResponse<List<DepartmentDto>>> GetAllDepartmentsAsync();

        Task<ApiResponse<DepartmentDto>> UpdateDepartmentAsync(UpdateDepartmentDto updateDepartmentDto);

        Task<ApiResponse<bool>> DeleteDepartmentAsync(int departmentId , Guid deletedByEmployeeId);




    }
}
