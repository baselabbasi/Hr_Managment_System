using HrMangmentSystem_Application.Common.Responses;
using HrMangmentSystem_Application.DTOs.Login;
using HrMangmentSystem_Application.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HrManagmentSystem_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        [AllowAnonymous]

        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var result = await _authenticationService.LoginAsync(loginRequestDto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("change-password")]
        [Authorize]

        public async Task<ActionResult<ApiResponse<bool>>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var employeeClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(employeeClaim) || !Guid.TryParse(employeeClaim, out var employeeId))
            {
                return Unauthorized();
            }

            var result = await _authenticationService.ChangePasswordAsync(employeeId, changePasswordDto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
