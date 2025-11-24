using AutoMapper;
using HrMangmentSystem_Application.Common;
using HrMangmentSystem_Application.DTOs.Department;
using HrMangmentSystem_Application.Interfaces;
using HrMangmentSystem_Domain.Entities.Employees;
using HrMangmentSystem_Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HrMangmentSystem_Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ILogger<DepartmentService> _logger;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Department, int> _departmentRepository;

        public DepartmentService(
            ILogger<DepartmentService> logger,
            IMapper mapper,
            IGenericRepository<Department, int> departmentRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _departmentRepository = departmentRepository;
        }
        public async Task<ApiResponse<DepartmentDto?>> CreateDepartmentAsync(CreateDepartmentDto createDepartmentDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createDepartmentDto.Code) || (string.IsNullOrWhiteSpace(createDepartmentDto.DeptName)))
                {
                    _logger.LogWarning("Create Department : DeptName and Code are required.");
                    return ApiResponse<DepartmentDto?>.Fail("DeptName and Code are required.");
                }

                // Check for unique of Department Code
                var existingDept = await _departmentRepository.FindAsync(d => d.Code == createDepartmentDto.Code);
                if (existingDept.Any())
                {
                    _logger.LogWarning($"Create Department : Department with Code {createDepartmentDto.Code} already exists.");
                    return ApiResponse<DepartmentDto?>.Fail($"Department with Code {createDepartmentDto.Code} already exists.");
                }

                if (createDepartmentDto.ParentDepartmentId.HasValue)
                {
                    var parentDept = await _departmentRepository.GetByIdAsync(createDepartmentDto.ParentDepartmentId.Value);
                    if (parentDept is null)
                    {
                        _logger.LogWarning($"Create Department : Parent Department with Id {createDepartmentDto.ParentDepartmentId.Value} not found.");
                        return ApiResponse<DepartmentDto?>.Fail($"Parent Department with Id {createDepartmentDto.ParentDepartmentId.Value} not found.");
                    }
                }

                var department = _mapper.Map<Department>(createDepartmentDto);

                await _departmentRepository.AddAsync(department);
                await _departmentRepository.SaveChangesAsync();

                var deptDto = _mapper.Map<DepartmentDto>(department);

                _logger.LogInformation($"Department with Id {deptDto.Id} created successfully.");
                return ApiResponse<DepartmentDto?>.Ok(deptDto, "Department created successfully.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating depatment");
                return ApiResponse<DepartmentDto?>.Fail("An error occurred while creating the department.");
            }
        }

        public async Task<ApiResponse<bool>> DeleteDepartmentAsync(int departmentId, Guid? deletedByEmployeeId)
        {
            try
            {
                var department = await _departmentRepository.GetByIdAsync(departmentId);
                if (department is null)
                {
                    _logger.LogWarning($"Delete Department: Department with Id {departmentId} not found");
                    return ApiResponse<bool>.Fail($"Department with Id {departmentId} not found");
                }

                await _departmentRepository.DeleteAsync(departmentId, deletedByEmployeeId);
                await _departmentRepository.SaveChangesAsync();

                _logger.LogInformation($"Department with Id {departmentId} deleted successfully");
                return ApiResponse<bool>.Ok(true, "Department deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting department");
                return ApiResponse<bool>.Fail("An error occurred while deleting the department.");
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
                return ApiResponse<List<DepartmentDto>>.Fail("An error occurred while retrieving departments.");
            }
        }

        public async Task<ApiResponse<DepartmentDto?>> GetDepartmentByIdAsync(int departmentId)
        {
            var department = await _departmentRepository.GetByIdAsync(departmentId);
            if (department is null)
            {
                _logger.LogWarning($"Get Department By Id: Department Id {departmentId} not found");
                return ApiResponse<DepartmentDto?>.Fail("Department not found");
            }
            var departmentDto = _mapper.Map<DepartmentDto>(department);

            return ApiResponse<DepartmentDto?>.Ok(departmentDto);
        }

        public async Task<ApiResponse<DepartmentDto>> UpdateDepartmentAsync(UpdateDepartmentDto updateDepartmentDto)
        {
            try
            {
                var department = await _departmentRepository.GetByIdAsync(updateDepartmentDto.Id);
                if (department is null)
                {
                    _logger.LogWarning($"Update Department: Department Id {updateDepartmentDto.Id} not found");
                    return ApiResponse<DepartmentDto>.Fail($"Department Id : {updateDepartmentDto.Id} not found");
                }
                // Check for unique of Department Code
                if (!string.IsNullOrWhiteSpace(updateDepartmentDto.Code) && updateDepartmentDto.Code != department.Code)
                {
                    var existingDept = await _departmentRepository.FindAsync(d => d.Code == updateDepartmentDto.Code && d.Id != updateDepartmentDto.Id);
                    if (existingDept.Any())
                    {
                        _logger.LogWarning($"Update Department : Department with Code {updateDepartmentDto.Code} already exists.");
                        return ApiResponse<DepartmentDto>.Fail($"Department with Code {updateDepartmentDto.Code} already exists.");
                    }
                }
                if (updateDepartmentDto.ParentDepartmentId.HasValue)
                {
                    // A department cannot be its own parent
                    if (updateDepartmentDto.ParentDepartmentId.Value == updateDepartmentDto.Id)
                    {
                        _logger.LogWarning("Update Department: A department cannot be its own parent");
                        return ApiResponse<DepartmentDto>.Fail("A department cannot be its own parent");
                    }

                    var parentDept = await _departmentRepository.GetByIdAsync(updateDepartmentDto.ParentDepartmentId.Value);
                    if (parentDept is null)
                    {
                        _logger.LogWarning($"Update Department: Parent Department Id {updateDepartmentDto.ParentDepartmentId} not found");
                        return ApiResponse<DepartmentDto>.Fail($"Parent Department Id : {updateDepartmentDto.ParentDepartmentId} not found");
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
                return ApiResponse<DepartmentDto>.Ok(deptDto, $"Department Id {updateDepartmentDto.Id} updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating department");
                return ApiResponse<DepartmentDto>.Fail("An error occurred while updating the department.");

            }
        }
    }
}
