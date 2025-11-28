using HrMangmentSystem_Application.Common.Responses;
using HrMangmentSystem_Application.DTOs.Login;

namespace HrMangmentSystem_Application.Interfaces.Auth
{
    public interface IAuthenticationService
    {
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto loginRequestDto);
    }
}
