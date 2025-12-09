using AutoMapper;
using HrManagmentSystem_Shared.Resources;
using HrMangmentSystem_Application.Common.PagedRequest;
using HrMangmentSystem_Application.Common.Responses;
using HrMangmentSystem_Application.DTOs.Requests.Generic;
using HrMangmentSystem_Application.DTOs.Requests.Leave;
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
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly IGenericRepository<GenericRequest, int> _genericRequestRepository;
        private readonly IGenericRepository<LeaveRequest, int> _leaveRequestRepository;
        private readonly IGenericRepository<RequestHistory, int> _requestHistoryRepository;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<LeaveRequestService> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IMapper _mapper;
        public LeaveRequestService(
            IGenericRepository<GenericRequest, int> genericRequestRepository,
            IGenericRepository<LeaveRequest, int> leaveRequestRepository,
            IGenericRepository<RequestHistory, int> requestHistoryRepository,
            ICurrentUser currentUser,
            ILogger<LeaveRequestService> logger,
            IStringLocalizer<SharedResource> localizer,
            IMapper mapper)
        {
            _genericRequestRepository = genericRequestRepository;
            _leaveRequestRepository = leaveRequestRepository;
            _requestHistoryRepository = requestHistoryRepository;
            _currentUser = currentUser;
            _logger = logger;
            _localizer = localizer;
            _mapper = mapper;
        }

        public async Task<ApiResponse<LeaveRequestDetailsDto>> CreateLeaveRequestAsync(CreateLeaveRequestDto createLeaveRequestDto)
        {
            try
            {
                var employeeId = _currentUser.EmployeeId;
                if (employeeId == Guid.Empty)
                {
                    _logger.LogWarning("Create LeaveRequest : Current User missing EmployeeId");
                    return ApiResponse<LeaveRequestDetailsDto>.Fail(_localizer["Auth_EmployeeNotLinked"]);
                }
                if (createLeaveRequestDto.StartDate > createLeaveRequestDto.EndDate)
                {
                    _logger.LogWarning("Create LeaveRequest: StartDate > EndDate");
                    return ApiResponse<LeaveRequestDetailsDto>.Fail(_localizer["LeaveRequest_InvalidDateRange"]);
                }
                if (createLeaveRequestDto.EndDate < DateTime.Now)
                {
                    _logger.LogWarning("Create LeaveRequest: EndDate is in the past");
                    return ApiResponse<LeaveRequestDetailsDto>.Fail(_localizer["LeaveRequest_EndDateInPast"]);
                }
                var totalDays = (createLeaveRequestDto.EndDate.Date - createLeaveRequestDto.StartDate.Date).Days + 1;
                if (totalDays <= 0)
                {
                    _logger.LogWarning("Create LeaveRequest: Calculated TotalDays <= 0");
                    return ApiResponse<LeaveRequestDetailsDto>.Fail(_localizer["LeaveRequest_InvalidTotalDays"]);
                }
                var overlappingExists = await _genericRequestRepository.Query(asNoTracking: true)
                    .Include(g => g.LeaveRequest)
                    .Where(g =>
                               g.RequestType == RequestType.LeaveRequest
                           && g.RequestedByEmployeeId == employeeId
                           && (g.RequestStatus == RequestStatus.Submitted ||
                                      g.RequestStatus == RequestStatus.Approved ||
                                      g.RequestStatus == RequestStatus.InReview)
                           && g.LeaveRequest != null
                           && g.LeaveRequest.StartDate.Date <= createLeaveRequestDto.EndDate.Date
                           && g.LeaveRequest.EndDate.Date >= createLeaveRequestDto.StartDate.Date)
                    .AnyAsync();

                if (overlappingExists)
                {
                    _logger.LogWarning($"Create LeaveRequest: Overlapping leave request for employee {employeeId}");

                    return ApiResponse<LeaveRequestDetailsDto>.Fail(_localizer["LeaveRequest_OverlappingDates"]);
                }

                var generic = _mapper.Map<GenericRequest>(createLeaveRequestDto);
                generic.RequestedByEmployeeId = employeeId;

                var leave = _mapper.Map<LeaveRequest>(createLeaveRequestDto);
                leave.TotalDays = totalDays;
                leave.GenericRequest = generic;

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
                await _leaveRequestRepository.AddAsync(leave);
                await _requestHistoryRepository.AddAsync(history);

                await _genericRequestRepository.SaveChangesAsync();


                var requestDto = _mapper.Map<GenericRequestListItemDto>(generic);
                var leaveDto = _mapper.Map<LeaveRequestDto>(leave);
                var historyDto = _mapper.Map<List<RequestHistoryDto>>(new[] { history });

                var details = new LeaveRequestDetailsDto
                {
                    History = historyDto,
                    Leave = leaveDto,
                    Request = requestDto
                };
                _logger.LogInformation($"Create LeaveRequest: Request {generic.Id} created by {employeeId}");

                return ApiResponse<LeaveRequestDetailsDto>.Ok(details, _localizer["LeaveRequest_Created"]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create LeaveRequest: Unexpected error");
                return ApiResponse<LeaveRequestDetailsDto>.Fail(_localizer["Generic_UnexpectedError"]);
            }
        }

        public async Task<ApiResponse<LeaveRequestDetailsDto?>> GetDetailsByIdAsync(int requestId)
        {
            try
            {
                var employeeId = _currentUser.EmployeeId;
                if (employeeId == Guid.Empty)
                {
                    _logger.LogWarning("Get Leave Request Details : Current User missing EmployeeId");
                    return ApiResponse<LeaveRequestDetailsDto?>.Fail(_localizer["Auth_EmployeeNotLinked"]);
                }
                var query = _genericRequestRepository.Query(asNoTracking: true)
                    .Include(g => g.RequestedByEmployee)
                    .Include(g => g.LeaveRequest)
                    .Include(g => g.History)
                    .ThenInclude(h => h.PerformedByEmployee)
                    .Where(g =>
                         g.Id == requestId && g.RequestType == RequestType.LeaveRequest);

                var generic = await query.FirstOrDefaultAsync();

                if (generic is null)
                {
                    _logger.LogWarning($"Get LeaveRequestDetails: Request {requestId} not found");
                    return ApiResponse<LeaveRequestDetailsDto?>.Fail(_localizer["Request_NotFound"]);
                }
                if (generic.RequestedByEmployeeId != employeeId && !_currentUser.Roles.Contains(RoleNames.HrAdmin))
                {
                    _logger.LogWarning($"Get LeaveRequestDetails: Forbidden for employee {employeeId}");
                    return ApiResponse<LeaveRequestDetailsDto?>.Fail(_localizer["Auth_Forbidden"]);
                }
                var requestDto = _mapper.Map<GenericRequestListItemDto>(generic);
                var leaveDto = _mapper.Map<LeaveRequestDto>(generic.LeaveRequest);

                var orderdHistory = generic.History
                    .OrderBy(h => h.PerformedAt)
                    .ToList();

                var historyDto = _mapper.Map<List<RequestHistoryDto>>(orderdHistory);

                var details = new LeaveRequestDetailsDto
                {
                    Request = requestDto,
                    Leave = leaveDto,
                    History = historyDto
                };
                return ApiResponse<LeaveRequestDetailsDto?>.Ok(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get LeaveRequestDetails: Unexpected error for RequestId {requestId}");

                return ApiResponse<LeaveRequestDetailsDto?>.Fail(_localizer["Generic_UnexpectedError"]);
            }
        }

        public async Task<ApiResponse<PagedResult<GenericRequestListItemDto>>> GetMyLeaveRequestsPagedAsync(PagedRequest request)
        {
            try
            {
                var employeeId = _currentUser.EmployeeId;
                if( employeeId == Guid.Empty )
                {
                    _logger.LogWarning("GetMyLeaveRequests: Current user missing EmployeeId");
                    return ApiResponse<PagedResult<GenericRequestListItemDto>>.Fail( _localizer["Auth_EmployeeNotLinked"]);
                }
                if (request.PageNumber <= 0)
                    request.PageNumber = 1;

                if (request.PageSize <= 0)
                    request.PageSize = 10;

                var query = _genericRequestRepository.Query(asNoTracking: true)
                  .Include(g => g.RequestedByEmployee) 
                  .Where(g =>
                      g.RequestType == RequestType.LeaveRequest && g.RequestedByEmployeeId == employeeId);
          

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

                _logger.LogInformation($"GetMyLeaveRequests: Loaded page {request.PageNumber} " +
                    $"(size {request.PageSize}) for employee {employeeId}");

                return ApiResponse<PagedResult<GenericRequestListItemDto>>.Ok(pagedResult, _localizer["LeaveRequest_ListLoaded"]);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetMyLeaveRequests: Unexpected error");
                return ApiResponse<PagedResult<GenericRequestListItemDto>>.Fail( _localizer["Generic_UnexpectedError"]);
            }
        }
    }
}
