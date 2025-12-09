using HrMangmentSystem_Application.Common.PagedRequest;
using HrMangmentSystem_Application.Common.Responses;
using HrMangmentSystem_Application.DTOs.Requests.Financial;
using HrMangmentSystem_Application.DTOs.Requests.Generic;
using HrMangmentSystem_Application.Interfaces.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/requests/financial")]
[Authorize]
public class FinancialRequestsController : ControllerBase
{
    private readonly IFinancialRequestService _service;

    public FinancialRequestsController(IFinancialRequestService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<FinancialRequestDetailsDto>>> CreateFinancialRequest(
        [FromBody] CreateFinancialRequestDto request)
    {
        var result = await _service.CreateFinancialRequestAsync(request);
        return Ok(result);
    }

    [HttpGet("{requestId:int}")]
    public async Task<ActionResult<ApiResponse<FinancialRequestDetailsDto?>>> GetFinancialRequestDetails(
        int requestId)
    {
        var result = await _service.GetDetailsByIdAsync(requestId);
        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<ActionResult<ApiResponse<PagedResult<GenericRequestListItemDto>>>> GetMyFinancialRequests(
        [FromQuery] PagedRequest request)
    {
        var result = await _service.GetMyFinancialRequestsPagedAsync(request);
        return Ok(result);
    }
}