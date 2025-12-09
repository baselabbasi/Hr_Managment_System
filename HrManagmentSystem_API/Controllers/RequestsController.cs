using HrMangmentSystem_Application.Common.PagedRequest;
using HrMangmentSystem_Application.Common.Responses;
using HrMangmentSystem_Application.DTOs.Requests.Generic;
using HrMangmentSystem_Application.Interfaces.Requests;
using HrMangmentSystem_Domain.Enum.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/requests")]
[Authorize]
public class RequestsController : ControllerBase
{
    private readonly IRequestService _requestService;

    public RequestsController(IRequestService requestService)
    {
        _requestService = requestService;
    }


    // Get requests created by current user (optionally filtered by type).
    [HttpGet("my")]
    public async Task<ActionResult<ApiResponse<PagedResult<GenericRequestListItemDto>>>> GetMyRequests(
        [FromQuery] PagedRequest request,
        [FromQuery] RequestType? type = null)
    {
        var result = await _requestService.GetMyRequestsPagedAsync(request, type);
        return Ok(result);
    }

    // Get requests pending for approval (HR / approver).
    [HttpGet("for-approval")]
    public async Task<ActionResult<ApiResponse<PagedResult<GenericRequestListItemDto>>>> GetRequestsForApproval(
        [FromQuery] PagedRequest request,
        [FromQuery] RequestType? type = null)
    {
        var result = await _requestService.GetRequestsForApprovalPagedAsync(request, type);
        return Ok(result);
    }

    
    // Get request header (basic info) by id.
    [HttpGet("{requestId:int}/header")]
    public async Task<ActionResult<ApiResponse<GenericRequestListItemDto?>>> GetRequestHeader(int requestId)
    {
        var result = await _requestService.GetRequestHeaderByIdAsync(requestId);
        return Ok(result);
    }

 
    // Get request history (status changes / comments / actions).
    [HttpGet("{requestId:int}/history")]
    public async Task<ActionResult<ApiResponse<List<RequestHistoryDto>>>> GetRequestHistory(int requestId)
    {
        var result = await _requestService.GetRequestHistoryAsync(requestId);
        return Ok(result);
    }

  
    // Change request status (Approve / Reject / Cancel / InReview)
    [HttpPost("change-status")]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeStatus([FromBody] ChangeRequestStatusDto dto)
    {
        var result = await _requestService.ChangeStatusAsync(dto);
        return Ok(result);
    }
}