using HrMangmentSystem_Application.Common;
using HrMangmentSystem_Application.DTOs.Employee;
using HrMangmentSystem_Application.Interfaces;
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
        public async Task<ActionResult<ApiResponse<List<EmployeeDto>>>> GetAllEmployees()
        {
            var result = await _employeeService.GetAllEmployeesAsync();
            return Ok(result);
        }

        // GET: api/Employees/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetEmployeeById(Guid id)
        {
            var result = await _employeeService.GetEmployeeByIdAsync(id);
            if (result.Data is null || !result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        // POST: api/Employees
        [HttpPost]
        public async Task<ActionResult<ApiResponse<EmployeeDto>>> CreateEmployee([FromBody] CreateEmployeeDto createEmployeeDto) 
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.ToDictionary(
                         k => k.Key,
                         v => v.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                         );
                return BadRequest(ApiResponse<EmployeeDto>.Fail("Invalid data", error: error));
            }

            var result = await _employeeService.CreateEmployeeAsync(createEmployeeDto);

            if (!result.Success || result.Data is null)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetEmployeeById), new { id = result.Data!.Id }, result);
        }


        [HttpPut("{id:guid}")] // update : api/employees/{id}
        public async Task<ActionResult<ApiResponse<EmployeeDto>>> UpdateEmployee(Guid id, [FromBody] UpdateEmployeeDto updateEmployeeDto)
        {
            if (id != updateEmployeeDto.Id)
            {
                return BadRequest(ApiResponse<bool>.Fail("ID mismatch"));
            }
            var result = await _employeeService.UpdateEmployeeAsync(updateEmployeeDto);

            if (!result.Success || result.Data is null)
            {
                if(result.Message?.Contains("not found" , StringComparison.OrdinalIgnoreCase) == true)
                    return NotFound(result);

                return BadRequest(result);
            }

            return Ok(result);
        }
     
        // DELETE: api/Employees/{id}
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteEmployee(Guid id, [FromQuery] Guid? deletedByEmployeeId)
        {
            if(deletedByEmployeeId == Guid.Empty || !deletedByEmployeeId.HasValue)
            {
                return BadRequest(ApiResponse<bool>.Fail("DeletedByEmployeeId is required"));
            }

            var result = await _employeeService.DeleteEmployeeAsync(id, deletedByEmployeeId.Value);
            
            if (!result.Success)
            {
                if (result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                    return NotFound(result);

                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
