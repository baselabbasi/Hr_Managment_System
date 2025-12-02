using AutoMapper;
using HrManagmentSystem_Shared.Resources;
using HrMangmentSystem_Application.Common.PagedRequest;
using HrMangmentSystem_Application.Common.Responses;
using HrMangmentSystem_Application.DTOs.Department;
using HrMangmentSystem_Application.Interfaces.Repositories;
using HrMangmentSystem_Application.Interfaces.Repository;
using HrMangmentSystem_Application.Interfaces.Services;
using HrMangmentSystem_Domain.Entities.Employees;
using Microsoft.EntityFrameworkCore;
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
        private readonly ICurrentUser _currentUser;

        public DepartmentService(
            ILogger<DepartmentService> logger,
            IMapper mapper,
            IGenericRepository<Department, int> departmentRepository,
            IGenericRepository<Employee, Guid> employeeRepository,
            IStringLocalizer<SharedResource> localizer,
            ICurrentUser currentUser)
        {

            _localizer = localizer;
            _logger = logger;
            _mapper = mapper;
            _departmentRepository = departmentRepository;
            _employeeRepository = employeeRepository;
            _currentUser = currentUser;
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

        public async Task<ApiResponse<bool>> DeleteDepartmentAsync(int departmentId)
        {
            try
            {
                var department = await _departmentRepository.GetByIdAsync(departmentId);
                if (department is null)
                {
                    _logger.LogWarning($"Delete Department: Department with Id {departmentId} not found");
                    return ApiResponse<bool>.Fail(_localizer["Department_NotFound", departmentId]);
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
                await _departmentRepository.DeleteAsync(departmentId );
                await _departmentRepository.SaveChangesAsync();

                var deletedEmployeeBy = _currentUser.EmployeeId;
                _logger.LogInformation($"Department with Id {departmentId} deleted successfully by {deletedEmployeeBy}");
                return ApiResponse<bool>.Ok(true, _localizer["Department_Deleted",departmentId]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting department");
                return ApiResponse<bool>.Fail(_localizer["Generic_UnexpectedError"]);
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

        public async Task<ApiResponse<PagedResult<DepartmentDto>>> GetDepartmentPagedAsync(PagedRequest request)
        {
            try
            {
                if (request.PageNumber <= 0)
                    request.PageNumber = 1;

                if (request.PageSize <= 0)
                    request.PageSize = 10;

                var query = _departmentRepository.Query();

                if (!string.IsNullOrWhiteSpace(request.Term))
                {
                    var term = request.Term.Trim().ToLower();

                    query = query.Where(d =>
                        (!string.IsNullOrEmpty(d.DeptName) && d.DeptName.ToLower().Contains(term)) ||
                        (!string.IsNullOrEmpty(d.Code) && d.Code.ToLower().Contains(term)) ||
                        (!string.IsNullOrEmpty(d.Location) && d.Location.ToLower().Contains(term)));
                }
                if (!string.IsNullOrWhiteSpace(request.SortBy))
                {
                    var sort = request.SortBy.Trim().ToLower();

                    query = sort switch
                    {
                        "deptname" => request.Desc
                        ? query.OrderByDescending(d => d.DeptName)
                        : query.OrderBy(d => d.DeptName),

                        "code" => request.Desc
                        ? query.OrderByDescending(d => d.Code)
                        : query.OrderBy(d => d.Code),

                        "location" => request.Desc
                        ? query.OrderByDescending(d => d.Location)
                        : query.OrderBy(d => d.Location),

                        _ => request.Desc
                        ? query.OrderByDescending(d => d.DeptName)
                        : query.OrderBy(d => d.DeptName)
                    };

                }
                else
                {
                    query = query.OrderBy(d => d.DeptName);
                }

                var totalCount = await query.CountAsync();

                var items = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var dtoItems = _mapper.Map<List<DepartmentDto>>(items);

                var pagedResult = new PagedResult<DepartmentDto>
                {
                    Items = dtoItems,
                    TotalCount = totalCount,
                    Page = request.PageNumber,
                    PageSize = request.PageSize

                };

                _logger.LogInformation($"Retrieved department page {request.PageNumber} with page size {request.PageSize}");


                return ApiResponse<PagedResult<DepartmentDto>>.Ok(pagedResult, _localizer["Department_ListLoaded"]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving department with pagination ");
                return ApiResponse<PagedResult<DepartmentDto>>.Fail(_localizer["Generic_UnexpectedError"]);
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
