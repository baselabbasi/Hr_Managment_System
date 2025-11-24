using AutoMapper;
using HrMangmentSystem_Application.Common;
using HrMangmentSystem_Application.DTOs.Employee;
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

        public async Task<ApiResponse<EmployeeDto?>> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto)
        {
            try
            {
               

                if (string.IsNullOrWhiteSpace(createEmployeeDto.FirstName) || string.IsNullOrWhiteSpace(createEmployeeDto.LastName) || string.IsNullOrWhiteSpace(createEmployeeDto.Email))
                {
                    _logger.LogWarning("Create Employee: Missing basic fields");
                    return  ApiResponse<EmployeeDto?>.Fail("Missing basic fields" );
                }

                if (createEmployeeDto.DateOfBirth >= DateTime.UtcNow.Date)
                {
                    _logger.LogWarning("Create Employee: Invalid Date of Birth");

                    return ApiResponse<EmployeeDto?>.Fail("Invalid Date of Birth");
                }
                if (createEmployeeDto.EmploymentStartDate > DateTime.UtcNow.AddDays(30))
                {
                    _logger.LogWarning("Create Employee: Invalid Employment Start Date");
                    return ApiResponse<EmployeeDto?>.Fail("Invalid Employment Start Date");
                }
                var ageAtStart = createEmployeeDto.EmploymentStartDate.Date.Year - createEmployeeDto.DateOfBirth.Date.Year;
                if ( ageAtStart < 18)
                {
                    _logger.LogWarning("Create Employee: Employee must be at least 18 years old");
                    return ApiResponse<EmployeeDto?>.Fail("Employee must be at least 18 years old" );
                }
                if (string.IsNullOrWhiteSpace(createEmployeeDto.Position))
                {
                    _logger.LogWarning("Create Employee: Position is required");
                    return ApiResponse<EmployeeDto?>.Fail("Position is required" );
                }



                var DepartmentId = await _departmentRepository.GetByIdAsync(createEmployeeDto.DepartmentId);
                if (DepartmentId is null)
                {
                    _logger.LogWarning($"Create Employee: Department Id {createEmployeeDto.DepartmentId} not found");
                    return ApiResponse<EmployeeDto?>.Fail($"Department Id {createEmployeeDto.DepartmentId} not found" );
                }

                var existingEmployee = await _employeeRepository.FindAsync(e => e.Email == createEmployeeDto.Email);
                if (existingEmployee.Any())
                {
                    _logger.LogWarning($"Create Employee: Employee with email {createEmployeeDto.Email} already exists");
                    return ApiResponse<EmployeeDto?>.Fail($"Employee with email {createEmployeeDto.Email} already exists" );
                }


                var employee = _mapper.Map<Employee>(createEmployeeDto);
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
                    return ApiResponse<bool>.Fail($"Employee Id {employeeId} not found");
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

                _logger.LogInformation("Retrieved all employees successfully");
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
    


        public async Task<ApiResponse<EmployeeDto>> UpdateEmployeeAsync(UpdateEmployeeDto updateDepartmentDto)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(updateDepartmentDto.Id);
                if (employee is null)
                {
                    _logger.LogWarning($"Update Employee: Employee Id {updateDepartmentDto.Id} not found");
                    return ApiResponse<EmployeeDto>.Fail($"Employee Id : {updateDepartmentDto.Id} not found");
                }
                if (updateDepartmentDto.DepartmentId.HasValue)
                {
                    var department = await _departmentRepository.GetByIdAsync(updateDepartmentDto.DepartmentId.Value);
                    if (department is null)
                    {
                        _logger.LogWarning($"Update Employee: Department Id {updateDepartmentDto.DepartmentId} not found");
                        return ApiResponse<EmployeeDto>.Fail($"Department Id {updateDepartmentDto.DepartmentId} not found");
                    }
                    var departmentId = updateDepartmentDto.DepartmentId.Value;
                }
               

                if (updateDepartmentDto.DepartmentId.HasValue)
                    employee.DepartmentId = updateDepartmentDto.DepartmentId.Value;

                if (!string.IsNullOrWhiteSpace(updateDepartmentDto.Position))
                    employee.Position = updateDepartmentDto.Position;

                if (!string.IsNullOrWhiteSpace(updateDepartmentDto.Address))
                    employee.Address = updateDepartmentDto.Address;


                _employeeRepository.Update(employee);
                await _employeeRepository.SaveChangesAsync();

                var employeeDto = _mapper.Map<EmployeeDto>(employee);

                return ApiResponse<EmployeeDto>.Ok(employeeDto, $"Employee Id {updateDepartmentDto.Id} updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating employee");
                return ApiResponse<EmployeeDto>.Fail("An error occurred while updating the employee");
            }

        }
    }
}
