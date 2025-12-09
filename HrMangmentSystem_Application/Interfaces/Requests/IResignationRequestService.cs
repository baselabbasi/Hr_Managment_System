using HrMangmentSystem_Application.Common.PagedRequest;
using HrMangmentSystem_Application.Common.Responses;
using HrMangmentSystem_Application.DTOs.Requests.Generic;
using HrMangmentSystem_Application.DTOs.Requests.Resignation;

namespace HrMangmentSystem_Application.Interfaces.Requests
{
    public interface IResignationRequestService
    {
        Task<ApiResponse<ResignationRequestDetailsDto>> CreateResignationRequestAsync(
                        CreateResignationRequestDto createResignationRequestDto);

        Task<ApiResponse<ResignationRequestDetailsDto?>> GetDetailsByIdAsync(int requestId);

        Task<ApiResponse<PagedResult<GenericRequestListItemDto>>> GetMyResignationRequestsPagedAsync(PagedRequest request);

    }   
}
