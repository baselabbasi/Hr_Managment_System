using AutoMapper;
using HrMangmentSystem_Application.Common;
using HrMangmentSystem_Application.DTOs;
using HrMangmentSystem_Application.Interfaces;
using HrMangmentSystem_Domain.Entities.Employees;
using HrMangmentSystem_Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace HrMangmentSystem_Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IGenericRepository<Employee, Guid> _employeeRepository;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Department, int> _departmentRepository;
        private readonly ILogger<EmployeeService> _logger;
        public EmployeeService(IGenericRepository<Employee, Guid> employeeRepository,
            IMapper mapper,
            IGenericRepository<Department, int> deparmentRepository,
            ILogger<EmployeeService> logger)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _departmentRepository = deparmentRepository;
            _logger = logger;

        }

        public async Task<ApiResponse<EmployeeDto?>> CreateEmployeeAsync(CreateEmployeeDto dto)
        {
            try
            {
               

                if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName) || string.IsNullOrWhiteSpace(dto.Email))
                {
                    _logger.LogWarning("Create Employee: Missing basic fields");
                    return  ApiResponse<EmployeeDto?>.Fail("Missing basic fields" );
                }

                if (dto.DateOfBirth >= DateTime.UtcNow.Date)
                {
                    _logger.LogWarning("Create Employee: Invalid Date of Birth");

                    return ApiResponse<EmployeeDto?>.Fail("Invalid Date of Birth");
                }
                if (dto.EmploymentStartDate > DateTime.UtcNow.AddDays(30))
                {
                    _logger.LogWarning("Create Employee: Invalid Employment Start Date");
                    return ApiResponse<EmployeeDto?>.Fail("Invalid Employment Start Date");
                }
                var ageAtStart = dto.EmploymentStartDate.Date.Year - dto.DateOfBirth.Date.Year;
                if ( ageAtStart < 18)
                {
                    _logger.LogWarning("Create Employee: Employee must be at least 18 years old");
                    return ApiResponse<EmployeeDto?>.Fail("Employee must be at least 18 years old" );
                }
                if (string.IsNullOrWhiteSpace(dto.Position))
                {
                    _logger.LogWarning("Create Employee: Position is required");
                    return ApiResponse<EmployeeDto?>.Fail("Position is required" );
                }



                var DepartmentId = await _departmentRepository.GetByIdAsync(dto.DepartmentId);
                if (DepartmentId is null)
                {
                    _logger.LogWarning($"Create Employee: Department Id {dto.DepartmentId} not found");
                    return ApiResponse<EmployeeDto?>.Fail($"Department Id {dto.DepartmentId} not found" );
                }

                var existingEmployee = await _employeeRepository.FindAsync(e => e.Email == dto.Email);
                if (existingEmployee.Any())
                {
                    _logger.LogWarning($"Create Employee: Employee with email {dto.Email} already exists");
                    return ApiResponse<EmployeeDto?>.Fail($"Employee with email {dto.Email} already exists" );
                }


                var employee = _mapper.Map<Employee>(dto);
                employee.Password = "Test@123"; // Default password to Test
               

                await _employeeRepository.AddAsync(employee);
                await _employeeRepository.SaveChangesAsync();
                 var employeeDto = _mapper.Map<EmployeeDto>(employee);
                 _logger.LogInformation($"Employee created successfully with Id {employee.Id}");

                return ApiResponse<EmployeeDto?>.Ok(employeeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating employee");
                 return ApiResponse<EmployeeDto?>.Fail("An error occurred while creating the employee");
            }
        }

        public async Task<ApiResponse<bool>> DeleteEmployeeAsync(Guid employeeId, Guid? deletedByEmployeeId)
        {

            try
            {
                var employee = await _employeeRepository.GetByIdAsync(employeeId);
                if (employee is null)
                {
                    _logger.LogWarning($"Delete Employee: Employee Id {employeeId} not found");
                    return ApiResponse<bool>.Fail("Employee not found");
                }

                await _employeeRepository.DeleteAsync(employeeId, deletedByEmployeeId);
                await _employeeRepository.SaveChangesAsync();

                return ApiResponse<bool>.Ok(true ,$"Delete Employee Id {employeeId} is done");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting employee");
                return ApiResponse<bool>.Fail("An error occurred while deleting the employee");
            }
        }

        public async Task<ApiResponse<List<EmployeeDto>>> GetAllEmployeesAsync()
        {
            try
            { 
                var employees = await _employeeRepository.GetAllAsync();
                 var employeeDtos = _mapper.Map<List<EmployeeDto>>(employees);
                return ApiResponse<List<EmployeeDto>>.Ok(employeeDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all employees");
                return ApiResponse<List<EmployeeDto>>.Fail("An error occurred while retrieving employees");

            }
        }

        public async Task<ApiResponse<EmployeeDto?>> GetEmployeeByIdAsync(Guid employeeId)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(employeeId);
                if (employee is null)
                {
                    _logger.LogWarning($"Get Employee By Id: Employee Id {employeeId} not found");
                    return ApiResponse<EmployeeDto?>.Fail("Employee not found");
                }

                var employeeDto = _mapper.Map<EmployeeDto>(employee);
                return ApiResponse<EmployeeDto?>.Ok(employeeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving employee by Id : {employeeId}");
                return ApiResponse<EmployeeDto?>.Fail("An error occurred while retrieving the employee");
            }
        }
    


        public async Task<ApiResponse<bool>> UpdateEmployeeAsync(UpdateEmployeeDto dto)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(dto.Id);
                if (employee is null)
                {
                    _logger.LogWarning($"Update Employee: Employee Id {dto.Id} not found");
                    return ApiResponse<bool>.Fail($"Employee Id : {dto.Id} not found");
                }
                if (dto.DepartmentId.HasValue)
                {
                    var department = await _departmentRepository.GetByIdAsync(dto.DepartmentId.Value);
                    if (department is null)
                    {
                        _logger.LogWarning($"Update Employee: Department Id {dto.DepartmentId} not found");
                        return ApiResponse<bool>.Fail($"Department Id {dto.DepartmentId} not found");
                    }
                    var departmentId = dto.DepartmentId.Value;
                }
                if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                    employee.PhoneNumber = dto.PhoneNumber;

                if (dto.DepartmentId.HasValue)
                    employee.DepartmentId = dto.DepartmentId.Value;

                if (!string.IsNullOrWhiteSpace(dto.Position))
                    employee.Position = dto.Position;

                if (!string.IsNullOrWhiteSpace(dto.Address))
                    employee.Address = dto.Address;


                _employeeRepository.Update(employee);
                await _employeeRepository.SaveChangesAsync();

                return ApiResponse<bool>.Ok(true, $"Employee Id {dto.Id} updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating employee");
                return ApiResponse<bool>.Fail("An error occurred while updating the employee");
            }

        }
    }
}
