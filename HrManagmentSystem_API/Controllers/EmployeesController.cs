using HrMangmentSystem_Application.Employees.DTOs;
using HrMangmentSystem_Application.Employees.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HrManagmentSystem_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController( IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        // GET: api/Employees
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }
        // GET: api/Employees/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetEmployeeById(Guid id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }
        // POST: api/Employees
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto createEmployeeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var employee = await _employeeService.CreateEmployeeAsync(createEmployeeDto);

            return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
        }

        [HttpPut("{id:guid}")] // update : api/employees/{id}
        public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] UpdateEmployeeDto updateEmployeeDto)
        {
            if (id != updateEmployeeDto.Id)
            {
                return BadRequest("ID mismatch");
            }
            var result = await _employeeService.UpdateEmployeeAsync(updateEmployeeDto);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        // DELETE: api/Employees/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteEmployee(Guid id, [FromQuery] Guid? deletedByEmployeeId)
        {
            if(deletedByEmployeeId == Guid.Empty)
            {
                return BadRequest("DeletedByEmployeeId is required");
            }
            var result = await _employeeService.DeleteEmployeeAsync(id, deletedByEmployeeId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
