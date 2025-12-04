using HrMangmentSystem_Application.Common.PagedRequest;
using HrMangmentSystem_Application.Common.Responses;
using HrMangmentSystem_Application.DTOs.Job.Appilcation;

namespace HrMangmentSystem_Application.Interfaces.Services
{
    public interface IJobApplicationService
    {
        Task<ApiResponse<JobApplicationDto>> ApplyAsync(int jobPositionId, CreateJobApplicationDto createJobApplicationDto);
        Task<ApiResponse<PagedResult<JobApplicationDto>>> GetByJobPositionPagedAsync(int jobPositionId , PagedRequest request);

    }
}
