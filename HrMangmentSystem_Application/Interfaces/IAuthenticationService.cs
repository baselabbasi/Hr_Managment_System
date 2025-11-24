using HrMangmentSystem_Application.DTOs.Login;

namespace HrMangmentSystem_Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto);


    }
}
