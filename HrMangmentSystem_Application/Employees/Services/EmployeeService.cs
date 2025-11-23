using AutoMapper;
using HrMangmentSystem_Application.Employees.DTOs;
using HrMangmentSystem_Application.Employees.Interfaces;
using HrMangmentSystem_Domain.Entities.Employees;
using HrMangmentSystem_Infrastructure.Repositories.Interfaces;

namespace HrMangmentSystem_Application.Employees.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IGenericRepository<Employee , Guid> _employeeRepository;
        private readonly IMapper _mapper;
        public EmployeeService(IGenericRepository<Employee, Guid> employeeRepository , IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;

        }

        public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto)
        {
         var employee = _mapper.Map<Employee>(createEmployeeDto);

            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveChangesAsync();

            return _mapper.Map<EmployeeDto>(employee);

        }

        public async Task<bool> DeleteEmployeeAsync(Guid employeeId, Guid? deletedByEmployeeId)
        {
           var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee is null)
                return false;
            
            await _employeeRepository.DeleteAsync(employeeId, deletedByEmployeeId);
            await _employeeRepository.SaveChangesAsync();

            return true;
        }

        public async Task<List<EmployeeDto>> GetAllEmployeesAsync()
        {
           var employees = await _employeeRepository.GetAllAsync();

            return _mapper.Map<List<EmployeeDto>>(employees);
        }

        public Task<EmployeeDto?> GetEmployeeByIdAsync(Guid employeeId)
        {
            var employee =  _employeeRepository.GetByIdAsync(employeeId);

            return _mapper.Map<Task<EmployeeDto?>>(employee);
        }

        public async Task<bool> UpdateEmployeeAsync(UpdateEmployeeDto updateEmployeeDto)
        {
          var employee = await _employeeRepository.GetByIdAsync(updateEmployeeDto.Id);
            if (employee is null)
                return false;
            if (!string.IsNullOrWhiteSpace(updateEmployeeDto.PhoneNumber))
                employee.PhoneNumber = updateEmployeeDto.PhoneNumber;

            if (updateEmployeeDto.DepartmentId.HasValue)
                employee.DepartmentId = updateEmployeeDto.DepartmentId.Value;

            if (!string.IsNullOrWhiteSpace(updateEmployeeDto.Position))
                employee.Position = updateEmployeeDto.Position;

            if (!string.IsNullOrWhiteSpace(updateEmployeeDto.Address))
                employee.Address = updateEmployeeDto.Address;


            _employeeRepository.Update(employee);
            await _employeeRepository.SaveChangesAsync();

            return true;
        }
    }
}
