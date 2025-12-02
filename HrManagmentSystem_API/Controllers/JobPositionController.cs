using HrMangmentSystem_Application.Common.PagedRequest;
using HrMangmentSystem_Application.Common.Responses;
using HrMangmentSystem_Application.DTOs.Job;
using HrMangmentSystem_Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrManagmentSystem_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class JobPositionController : ControllerBase
    {
        private readonly IJobPositionService _jobPositionService;

        public JobPositionController(IJobPositionService jobPositionService)
        {
            _jobPositionService = jobPositionService;
        }


        [HttpGet]
        public async Task<ActionResult<ApiResponse<JobPositionDto>>> GetAllJobPosition([FromQuery] PagedRequest request)
        {
            var result = await _jobPositionService.GetJobPositionsPagedAsync(request);

            return Ok(result);
        }
        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<JobPositionDto>>> GetJobPositionById(int id)
        {
            var result = await _jobPositionService.GetJobPositionsByIdAsync(id);
            if (result.Data is null || !result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

       
        [HttpPost]
        public async Task<ActionResult<ApiResponse<JobPositionDto>>> CreateJobPosition([FromBody] CreateJobPositionDto createJobPositionDto)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.ToDictionary(
                         k => k.Key,
                         v => v.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                         );
                return BadRequest(ApiResponse<JobPositionDto>.Fail("Invalid data", error: error));
            }

            var result = await _jobPositionService.CreateJobPositionsAsync(createJobPositionDto);

            if (!result.Success || result.Data is null)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetJobPositionById), new { id = result.Data!.Id }, result);
        }


        [HttpPut("{id:int}")] 
        public async Task<ActionResult<ApiResponse<JobPositionDto>>> UpdateJobPosition(int id, [FromBody] UpdateJobPositionDto updateJobPositionDto)
        {
            if (id != updateJobPositionDto.Id)
            {
                return BadRequest(ApiResponse<bool>.Fail("ID mismatch"));
            }
            var result = await _jobPositionService.UpdateJobPositionsAsync(updateJobPositionDto);

            if (!result.Success || result.Data is null)
            {
                if (result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                    return NotFound(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteJobPosition(int jobPositionId)
        {


            var result = await _jobPositionService.DeleteJobPositionsAsync(jobPositionId);

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
