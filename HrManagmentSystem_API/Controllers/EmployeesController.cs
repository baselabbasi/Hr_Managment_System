using HrMangmentSystem_Application.Common;
using HrMangmentSystem_Application.DTOs;
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
        public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetAllEmployees()
        {
            var result = await _employeeService.GetAllEmployeesAsync();
            return Ok(result);
        }

        // GET: api/Employees/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetEmployeeById(Guid id)
        {
            var result = await _employeeService.GetEmployeeByIdAsync(id);
            if (result is null || !result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        // POST: api/Employees
        [HttpPost]
        public async Task<ActionResult<ApiResponse<EmployeeDto>>> CreateEmployee([FromBody] CreateEmployeeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<EmployeeDto>.Fail("Invalid data",
                    error: ModelState.ToDictionary(
                        k => k.Key,
                        v => v.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        )));

            var result = await _employeeService.CreateEmployeeAsync(dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetEmployeeById), new { id = result.Data!.Id }, result);
        }


        [HttpPut("{id:guid}")] // update : api/employees/{id}
        public async Task<ActionResult<ApiResponse<EmployeeDto>>> UpdateEmployee(Guid id, [FromBody] UpdateEmployeeDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(ApiResponse<bool>.Fail("ID mismatch"));
            }
            var result = await _employeeService.UpdateEmployeeAsync(dto);

            if (!result.Success)
            {
                if(result.Message?.Contains("not found" , StringComparison.OrdinalIgnoreCase) == true)
                    return NotFound(result);

                return BadRequest(result);
            }

            return Ok(result);
        }
     
        // DELETE: api/Employees/{id}
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<EmployeeDto>>> DeleteEmployee(Guid id, [FromQuery] Guid? deletedByEmployeeId)
        {
            if(deletedByEmployeeId == Guid.Empty)
            {
                return BadRequest(ApiResponse<bool>.Fail("DeletedByEmployeeId is required"));
            }
            var result = await _employeeService.DeleteEmployeeAsync(id, deletedByEmployeeId);
            
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
