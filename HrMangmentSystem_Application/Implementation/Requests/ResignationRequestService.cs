using AutoMapper;
using HrManagmentSystem_Shared.Resources;
using HrMangmentSystem_Application.Common.PagedRequest;
using HrMangmentSystem_Application.Common.Responses;
using HrMangmentSystem_Application.DTOs.Requests.Generic;
using HrMangmentSystem_Application.DTOs.Requests.Resignation;
using HrMangmentSystem_Application.Interfaces.Repositories;
using HrMangmentSystem_Application.Interfaces.Repository;
using HrMangmentSystem_Application.Interfaces.Requests;
using HrMangmentSystem_Domain.Constants;
using HrMangmentSystem_Domain.Entities.Requests;
using HrMangmentSystem_Domain.Enum.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace HrMangmentSystem_Application.Implementation.Requests
{
    public class ResignationRequestService : IResignationRequestService
    {
        private readonly IGenericRepository<GenericRequest, int> _genericRequestRepository;
        private readonly IGenericRepository<ResignationRequest, int> _resignationRequestRepository;
        private readonly IGenericRepository<RequestHistory, int> _requestHistoryRepository;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<ResignationRequestService> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IMapper _mapper;

        public ResignationRequestService(
            IGenericRepository<GenericRequest, int> genericRequestRepository,
            IGenericRepository<ResignationRequest, int> resignationRequestRepository,
            IGenericRepository<RequestHistory, int> requestHistoryRepository,
            ICurrentUser currentUser,
            ILogger<ResignationRequestService> logger,
            IStringLocalizer<SharedResource> localizer,
            IMapper mapper)
        {
            _genericRequestRepository = genericRequestRepository;
            _resignationRequestRepository = resignationRequestRepository;
            _requestHistoryRepository = requestHistoryRepository;
            _currentUser = currentUser;
            _logger = logger;
            _localizer = localizer;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ResignationRequestDetailsDto>> CreateResignationRequestAsync(
            CreateResignationRequestDto createResignationRequestDto)
        {
            try
            {
                var employeeId = _currentUser.EmployeeId;
                if (employeeId == Guid.Empty)
                {
                    _logger.LogWarning("Create ResignationRequest: Current user missing EmployeeId");
                    return ApiResponse<ResignationRequestDetailsDto>.Fail(_localizer["Auth_EmployeeNotLinked"]);
                }

                if (createResignationRequestDto.ProposedLastWorkingDate.Date < DateTime.Now.Date)
                {
                    _logger.LogWarning("Create ResignationRequest: ProposedLastWorkingDate in the past ({Date})",
                        createResignationRequestDto.ProposedLastWorkingDate);
                    return ApiResponse<ResignationRequestDetailsDto>.Fail(
                        _localizer["ResignationRequest_InvalidLastWorkingDate"]);
                }

                var generic = _mapper.Map<GenericRequest>(createResignationRequestDto);
                generic.RequestedByEmployeeId = employeeId;


                var resignation = _mapper.Map<ResignationRequest>(createResignationRequestDto);
                resignation.GenericRequest = generic;

                var history = new RequestHistory
                {
                    GenericRequest = generic,
                    Action = RequestAction.Submitted,
                    OldStatus = null,
                    NewStatus = RequestStatus.Submitted,
                    Comment = null,
                    PerformedByEmployeeId = employeeId,
                    PerformedAt = DateTime.Now
                };

                await _genericRequestRepository.AddAsync(generic);
                await _resignationRequestRepository.AddAsync(resignation);
                await _requestHistoryRepository.AddAsync(history);

                await _genericRequestRepository.SaveChangesAsync();

                var requestDto = _mapper.Map<GenericRequestListItemDto>(generic);
                var resignationDto = _mapper.Map<ResignationRequestDto>(resignation);
                var historyDto = _mapper.Map<List<RequestHistoryDto>>(new[] { history });

                var details = new ResignationRequestDetailsDto
                {
                    Request = requestDto,
                    Resignation = resignationDto,
                    History = historyDto
                };

                _logger.LogInformation($"Create ResignationRequest: Request {generic.Id} created by {employeeId}");

                return ApiResponse<ResignationRequestDetailsDto>.Ok(details, _localizer["ResignationRequest_Created"]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create ResignationRequest: Unexpected error");
                return ApiResponse<ResignationRequestDetailsDto>.Fail(_localizer["Generic_UnexpectedError"]);
            }
        }

        public async Task<ApiResponse<ResignationRequestDetailsDto?>> GetDetailsByIdAsync(int requestId)
        {
            try
            {
                var employeeId = _currentUser.EmployeeId;
                if (employeeId == Guid.Empty)
                {
                    _logger.LogWarning("Get ResignationRequest Details: Current user missing EmployeeId");
                    return ApiResponse<ResignationRequestDetailsDto?>.Fail(_localizer["Auth_EmployeeNotLinked"]);
                }

                var query = _genericRequestRepository.Query(asNoTracking: true)
                    .Include(g => g.RequestedByEmployee)
                    .Include(g => g.ResignationRequest)
                    .Include(g => g.History)
                        .ThenInclude(h => h.PerformedByEmployee)
                    .Where(g =>
                         g.Id == requestId && g.RequestType == RequestType.ResignationRequest);

                var generic = await query.FirstOrDefaultAsync();

                if (generic is null)
                {
                    _logger.LogWarning($"Get ResignationRequestDetails: Request {requestId} not found");
                    return ApiResponse<ResignationRequestDetailsDto?>.Fail(_localizer["Request_NotFound"]);
                }

                if (generic.RequestedByEmployeeId != employeeId && !_currentUser.Roles.Contains(RoleNames.HrAdmin))
                {
                    _logger.LogWarning($"Get ResignationRequestDetails: Forbidden for employee {employeeId}");
                    return ApiResponse<ResignationRequestDetailsDto?>.Fail(_localizer["Auth_Forbidden"]);
                }

                var requestDto = _mapper.Map<GenericRequestListItemDto>(generic);
                var resignationDto = _mapper.Map<ResignationRequestDto>(generic.ResignationRequest);

                var orderedHistory = generic.History
                    .OrderBy(h => h.PerformedAt)
                    .ToList();

                var historyDto = _mapper.Map<List<RequestHistoryDto>>(orderedHistory);

                var details = new ResignationRequestDetailsDto
                {
                    Request = requestDto,
                    Resignation = resignationDto,
                    History = historyDto
                };

                return ApiResponse<ResignationRequestDetailsDto?>.Ok(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"Get ResignationRequestDetails: Unexpected error for RequestId {requestId}");

                return ApiResponse<ResignationRequestDetailsDto?>.Fail(_localizer["Generic_UnexpectedError"]);
            }
        }

        public async Task<ApiResponse<PagedResult<GenericRequestListItemDto>>> GetMyResignationRequestsPagedAsync(
            PagedRequest request)
        {
            try
            {
                var employeeId = _currentUser.EmployeeId;
                if (employeeId == Guid.Empty)
                {
                    _logger.LogWarning("GetMyResignationRequests: Current user missing EmployeeId");
                    return ApiResponse<PagedResult<GenericRequestListItemDto>>.Fail(_localizer["Auth_EmployeeNotLinked"]);
                }

                if (request.PageNumber <= 0) request.PageNumber = 1;
                if (request.PageSize <= 0) request.PageSize = 10;

                var query = _genericRequestRepository.Query(asNoTracking: true)
                  .Include(g => g.RequestedByEmployee)
                  .Where(g =>
                      g.RequestType == RequestType.ResignationRequest && g.RequestedByEmployeeId == employeeId);

                if (!string.IsNullOrWhiteSpace(request.Term))
                {
                    var term = request.Term.Trim().ToLower();

                    query = query.Where(g =>
                        (!string.IsNullOrEmpty(g.Title) && g.Title.ToLower().Contains(term)) ||
                        (!string.IsNullOrEmpty(g.Description) && g.Description.ToLower().Contains(term)));
                }

                if (!string.IsNullOrWhiteSpace(request.SortBy))
                {
                    var sort = request.SortBy.Trim().ToLower();

                    query = sort switch
                    {
                        "requestedat" => request.Desc
                            ? query.OrderByDescending(g => g.RequestedAt)
                            : query.OrderBy(g => g.RequestedAt),

                        "status" => request.Desc
                            ? query.OrderByDescending(g => g.RequestStatus)
                            : query.OrderBy(g => g.RequestStatus),

                        _ => request.Desc
                            ? query.OrderByDescending(g => g.RequestedAt)
                            : query.OrderBy(g => g.RequestedAt)
                    };
                }
                else
                {
                    query = query.OrderByDescending(g => g.RequestedAt);
                }

                var totalCount = await query.CountAsync();

                var items = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var dtoItems = _mapper.Map<List<GenericRequestListItemDto>>(items);

                var pagedResult = new PagedResult<GenericRequestListItemDto>
                {
                    Items = dtoItems,
                    TotalCount = totalCount,
                    Page = request.PageNumber,
                    PageSize = request.PageSize
                };

                _logger.LogInformation($"GetMyResignationRequests: Loaded page {request.PageNumber}" +
                    $" (size {request.PageSize}) for employee {employeeId}");

                return ApiResponse<PagedResult<GenericRequestListItemDto>>.Ok(
                    pagedResult,
                    _localizer["ResignationRequest_ListLoaded"]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetMyResignationRequests: Unexpected error");
                return ApiResponse<PagedResult<GenericRequestListItemDto>>.Fail(_localizer["Generic_UnexpectedError"]);
            }
        }
    }
}
