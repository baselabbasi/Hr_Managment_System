using AutoMapper;
using HrManagmentSystem_Shared.Common.Resources;
using HrMangmentSystem_Application.Common.Responses;
using HrMangmentSystem_Application.DTOs.Department;
using HrMangmentSystem_Application.Interfaces;
using HrMangmentSystem_Domain.Entities.Employees;
using HrMangmentSystem_Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace HrMangmentSystem_Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ILogger<DepartmentService> _logger;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Department, int> _departmentRepository;
        private readonly IGenericRepository<Employee, Guid> _employeeRepository;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public DepartmentService(
            ILogger<DepartmentService> logger,
            IMapper mapper,
            IGenericRepository<Department, int> departmentRepository,
            IGenericRepository<Employee, Guid> employeeRepository,
            IStringLocalizer<SharedResource> localizer
            )
        {

            _localizer = localizer;
            _logger = logger;
            _mapper = mapper;
            _departmentRepository = departmentRepository;
            _employeeRepository = employeeRepository;

        }
        public async Task<ApiResponse<DepartmentDto?>> CreateDepartmentAsync(CreateDepartmentDto createDepartmentDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createDepartmentDto.Code) || (string.IsNullOrWhiteSpace(createDepartmentDto.DeptName)))
                {
                    _logger.LogWarning("Create Department : DeptName and Code are required.");
                    return ApiResponse<DepartmentDto?>.Fail(_localizer["Department_CodeRequired"]);
                }

                // Check for unique of Department Code
                var existingDept = await _departmentRepository.FindAsync(d => d.Code == createDepartmentDto.Code);
                if (existingDept.Any())
                {
                    _logger.LogWarning($"Create Department : Department with Code {createDepartmentDto.Code} already exists.");
                    return ApiResponse<DepartmentDto?>.Fail(_localizer["Department_CodeExists", createDepartmentDto.Code]);
                }

                if (createDepartmentDto.ParentDepartmentId.HasValue)
                {
                    var parentDept = await _departmentRepository.GetByIdAsync(createDepartmentDto.ParentDepartmentId.Value);
                    if (parentDept is null)
                    {
                        _logger.LogWarning($"Create Department : Parent Department with Id {createDepartmentDto.ParentDepartmentId.Value} not found.");
                        return ApiResponse<DepartmentDto?>.Fail(_localizer["Department_ParentNotFound", createDepartmentDto.ParentDepartmentId.Value]);
                    }
                }

                var department = _mapper.Map<Department>(createDepartmentDto);

                await _departmentRepository.AddAsync(department);
                await _departmentRepository.SaveChangesAsync();

                var deptDto = _mapper.Map<DepartmentDto>(department);

                _logger.LogInformation($"Department with Id {deptDto.Id} created successfully.");
                return ApiResponse<DepartmentDto?>.Ok(deptDto, _localizer["Department_Created"]);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating depatment");
                return ApiResponse<DepartmentDto?>.Fail(_localizer["Generic_UnexpectedError"]);
            }
        }

        public async Task<ApiResponse<bool>> DeleteDepartmentAsync(int departmentId, Guid deletedByEmployeeId)
        {
            try
            {
                var department = await _departmentRepository.GetByIdAsync(departmentId);
                if (department is null)
                {
                    _logger.LogWarning($"Delete Department: Department with Id {departmentId} not found");
                    return ApiResponse<bool>.Fail(_localizer["Department_NotFound", departmentId]);
                }

                if (deletedByEmployeeId == Guid.Empty)
                {
                    _logger.LogWarning($"Delete Department: deleted {departmentId} DeletedBy is Required ");
                    return ApiResponse<bool>.Fail(_localizer["Delete_DeletedByRequired"]);
                }

                var hasEmployees = await _employeeRepository.FindAsync(e => e.DepartmentId == departmentId);
                if (hasEmployees.Any())
                {
                    _logger.LogWarning($"Delete Department : Department {departmentId} is has employee ");
                    return ApiResponse<bool>.Fail(_localizer["Department_HasEmployees", departmentId]);
                }

                var hasDepartment = await _departmentRepository.FindAsync(d => d.ParentDepartmentId == departmentId);
                if (hasDepartment.Any())
                {
                    _logger.LogWarning($"Delete Department : Department {departmentId} is has department children ");
                    return ApiResponse<bool>.Fail(_localizer["Department_HasChildren", departmentId]);
                }
                await _departmentRepository.DeleteAsync(departmentId, deletedByEmployeeId);
                await _departmentRepository.SaveChangesAsync();

                _logger.LogInformation($"Department with Id {departmentId} deleted successfully by {deletedByEmployeeId}");
                return ApiResponse<bool>.Ok(true, _localizer["Department_Deleted",departmentId]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting department");
                return ApiResponse<bool>.Fail(_localizer["Generic_UnexpectedError"]);
            }
        }

        public async Task<ApiResponse<List<DepartmentDto>>> GetAllDepartmentsAsync()
        {
            try
            {
                var departments = await _departmentRepository.GetAllAsync();
                var departmentDtos = _mapper.Map<List<DepartmentDto>>(departments);

                _logger.LogInformation("Retrieved all departments successfully");
                return ApiResponse<List<DepartmentDto>>.Ok(departmentDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all departments");
                return ApiResponse<List<DepartmentDto>>.Fail(_localizer["Generic_UnexpectedError"]);
            }
        }

        public async Task<ApiResponse<DepartmentDto?>> GetDepartmentByIdAsync(int departmentId)
        {
            try
            {
                var department = await _departmentRepository.GetByIdAsync(departmentId);
                if (department is null)
                {
                    _logger.LogWarning($"Get Department By Id: Department Id {departmentId} not found");
                    return ApiResponse<DepartmentDto?>.Fail(_localizer["Department_NotFound", departmentId]);
                }
                var departmentDto = _mapper.Map<DepartmentDto>(department);

                return ApiResponse<DepartmentDto?>.Ok(departmentDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving {departmentId} departments");
                return ApiResponse<DepartmentDto?>.Fail(_localizer["Generic_UnexpectedError"]);
            }
         }

        public async Task<ApiResponse<DepartmentDto>> UpdateDepartmentAsync(UpdateDepartmentDto updateDepartmentDto)
        {
            try
            {
                var department = await _departmentRepository.GetByIdAsync(updateDepartmentDto.Id);
                if (department is null)
                {
                    _logger.LogWarning($"Update Department: Department Id {updateDepartmentDto.Id} not found");
                    return ApiResponse<DepartmentDto>.Fail(_localizer["Department_NotFound", updateDepartmentDto.Id]);
                }
                // Check for unique of Department Code
                if (!string.IsNullOrWhiteSpace(updateDepartmentDto.Code) && updateDepartmentDto.Code != department.Code)
                {
                    var existingDept = await _departmentRepository.FindAsync(d => d.Code == updateDepartmentDto.Code && d.Id != updateDepartmentDto.Id);
                    if (existingDept.Any())
                    {
                        _logger.LogWarning($"Update Department : Department with Code {updateDepartmentDto.Code} already exists.");
                        return ApiResponse<DepartmentDto>.Fail(_localizer["Department_CodeExists", updateDepartmentDto.Code]);
                    }
                }
                if (updateDepartmentDto.ParentDepartmentId.HasValue)
                {
                    // A department cannot be its own parent
                    if (updateDepartmentDto.ParentDepartmentId.Value == updateDepartmentDto.Id)
                    {
                        _logger.LogWarning("Update Department: A department cannot be its own parent");
                        return ApiResponse<DepartmentDto>.Fail(_localizer["Department_CannotBeOwnParent"]);
                    }

                    var parentDept = await _departmentRepository.GetByIdAsync(updateDepartmentDto.ParentDepartmentId.Value);
                    if (parentDept is null)
                    {
                        _logger.LogWarning($"Update Department: Parent Department Id {updateDepartmentDto.ParentDepartmentId} not found");
                        return ApiResponse<DepartmentDto>.Fail(_localizer["Department_ParentNotFound"]);
                    }
                }
                if(!string.IsNullOrWhiteSpace(updateDepartmentDto.Code))
                    department.Code = updateDepartmentDto.Code;

                if (!string.IsNullOrWhiteSpace(updateDepartmentDto.DeptName))
                    department.DeptName = updateDepartmentDto.DeptName;

                if (!string.IsNullOrWhiteSpace(updateDepartmentDto.Description))
                    department.Description = updateDepartmentDto.Description;

                if (!string.IsNullOrWhiteSpace(updateDepartmentDto.Location))
                    department.Location = updateDepartmentDto.Location;

                department.ParentDepartmentId = updateDepartmentDto.ParentDepartmentId;

                _departmentRepository.Update(department);
                await _departmentRepository.SaveChangesAsync();

                var deptDto = _mapper.Map<DepartmentDto>(department);
                _logger.LogInformation($"Department with Id {deptDto.Id} updated successfully.");
                return ApiResponse<DepartmentDto>.Ok(deptDto, _localizer["Department_Updated"]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating department");
                return ApiResponse<DepartmentDto>.Fail(_localizer["Generic_UnexpectedError"]);

            }
        }
    }
}
