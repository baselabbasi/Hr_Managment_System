using HrManagmentSystem_Shared.Common.Resources;
using HrMangmentSystem_Application.Common.Responses;
using HrMangmentSystem_Application.DTOs.Login;
using HrMangmentSystem_Application.Interfaces.Auth;
using HrMangmentSystem_Application.Interfaces.Repositories;
using HrMangmentSystem_Domain.Entities.Employees;
using HrMangmentSystem_Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace HrMangmentSystem_Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IGenericRepository<Employee, Guid> _employeeRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IGenericRepository<EmployeeRole , int> _employeeRoleRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;


        public AuthenticationService(IGenericRepository<Employee, Guid> employeeRepository,
                   ITenantRepository tenantRepository,
                   IGenericRepository<EmployeeRole, int> employeeRoleRepository,
                 IJwtTokenGenerator jwtTokenGenerator,
                   ILogger<AuthenticationService> logger,
                   IStringLocalizer<SharedResource> localizer)
        {
            _employeeRepository = employeeRepository;
            _employeeRoleRepository = employeeRoleRepository;
            _tenantRepository = tenantRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
            _localizer = localizer;

        }

        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto loginRequestDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginRequestDto.Email) ||
                    string.IsNullOrWhiteSpace(loginRequestDto.Password) ||
                    string.IsNullOrWhiteSpace(loginRequestDto.TenantCode))
                {
                    _logger.LogWarning("Login failed : missing Tenant Code or Email or Password");
                    return ApiResponse<LoginResponseDto>.Fail(_localizer["Auth_InvalidCredentials"]);
                }

                var tenantCode = loginRequestDto.TenantCode.Trim();

                var tenant = await _tenantRepository.FindByCodeAsync(tenantCode);
                if (tenant is null || !tenant.IsActive)
                {
                    _logger.LogWarning($"Login failed: Tenant with code {tenantCode} not found or inactive");
                    return ApiResponse<LoginResponseDto>.Fail(_localizer["Auth_InvalidCredentials"]);
                }
                var employees = await _employeeRepository.FindAsync(e =>
                e.Email == loginRequestDto.Email &&
                e.TenantId == tenant.Id);

                var employee = employees.FirstOrDefault();
                if (employee is null)
                {
                    _logger.LogWarning($"Login failed: Employee with email {loginRequestDto.Email} not found in Tenant {tenant.Id}");
                    return ApiResponse<LoginResponseDto>.Fail(_localizer["Auth_InvalidCredentials"]);
                }

                var query = _employeeRoleRepository.Query()
                    .Where(er => er.EmployeeId == employee.Id)
                    .Include(er => er.Role);

                var roleNames = await query
                    .Select(er => er.Role.Name)
                    .ToListAsync();

               

                var (token , expiresAt) = _jwtTokenGenerator.GenerateToken(employee,tenant , roleNames);

                var responseDto = new LoginResponseDto
                {
                    Token = token,
                    Expiration = expiresAt,
                    EmployeeId = employee.Id,
                    FullName = $"{employee.FirstName} {employee.LastName}",
                    Email = employee.Email,
                    TenantId = tenant.Id,
                    Roles = roleNames,
                };

                _logger.LogInformation($"Login succeeded for {employee.Email} in tenant {tenant.Id}");

                return ApiResponse<LoginResponseDto>.Ok(responseDto, _localizer["Auth_LoginSuccess"]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error occurred during login for email {loginRequestDto.Email}");
                return ApiResponse<LoginResponseDto>.Fail(_localizer["Generic.UnexpectedError"]);

            }
        }

      
    }
}

