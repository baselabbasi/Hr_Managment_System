using HrMangmentSystem_Application.Common.PagedRequest;
using HrMangmentSystem_Application.Common.Responses;
using HrMangmentSystem_Application.DTOs.Job;

namespace HrMangmentSystem_Application.Interfaces.Services
{
    public interface IJobPositionService
    {
        Task<ApiResponse<JobPositionsDto>> CreateJobPositionsAsync(CreateJobPositionDto createJobPositionDto);

        Task<ApiResponse<JobPositionsDto?>> GetJobPositionsByIdAsync(int jobPositionId);

        Task<ApiResponse<PagedResult<JobPositionsDto>>> GetJobPositionsPagedAsync(PagedRequest request);

        Task<ApiResponse<JobPositionsDto>> UpdateJobPositionsAsync(UpdateJobPositionDto updateJobPositionDto);

        Task<ApiResponse<bool>> DeleteJobPositionsAsync(int jobPositionId);
    }
}
