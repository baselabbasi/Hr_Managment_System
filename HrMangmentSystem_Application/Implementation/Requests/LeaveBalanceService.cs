using AutoMapper;
using HrManagmentSystem_Shared.Enum.Request;
using HrManagmentSystem_Shared.Resources;
using HrMangmentSystem_Application.Common.Responses;
using HrMangmentSystem_Application.Interfaces.Requests;
using HrMangmentSystem_Domain.Entities.Employees;
using HrMangmentSystem_Dto.DTOs.Requests.EmployeeData;
using HrMangmentSystem_Infrastructure.Interfaces.Repositories;
using HrMangmentSystem_Infrastructure.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace HrMangmentSystem_Application.Implementation.Requests
{
    public class LeaveBalanceService : ILeaveBalanceService
    {
        private readonly IGenericRepository<EmployeeLeaveBalance, int> _leaveBalanceRepository;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<LeaveBalanceService> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IMapper _mapper;
        public LeaveBalanceService(
           IGenericRepository<EmployeeLeaveBalance, int> leaveBalanceRepository,
           ICurrentUser currentUser,
           ILogger<LeaveBalanceService> logger,
           IStringLocalizer<SharedResource> localizer,
           IMapper mapper)
        {
            _leaveBalanceRepository = leaveBalanceRepository;
            _currentUser = currentUser;
            _logger = logger;
            _localizer = localizer;
            _mapper = mapper;
        }
   
        public async Task<ApiResponse<List<EmployeeLeaveBalanceDto>>> GetMyLeaveBalanceAsync()
        {
            try
            {
                var employeeId = _currentUser.EmployeeId;
                if (employeeId == Guid.Empty)
                {
                    _logger.LogWarning("GetMyLeaveBalance: Current user missing EmployeeId");
                    return ApiResponse<List<EmployeeLeaveBalanceDto>>.Fail(_localizer["Auth_EmployeeNotLinked"]);
                }

                var query = _leaveBalanceRepository
                    .Query(asNoTracking: true)
                    .Where(b => b.EmployeeId == employeeId);

                var items = await query.ToListAsync();

                var dto = _mapper.Map<List<EmployeeLeaveBalanceDto>>(items);

                _logger.LogInformation("GetMyLeaveBalance: Loaded {Count} leave balance records for employee {EmployeeId}"
                    , dto.Count, employeeId);
                return ApiResponse<List<EmployeeLeaveBalanceDto>>.Ok(dto,_localizer["LeaveBalance_MyLoaded"]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetMyLeaveBalance: Unexpected error");

                return ApiResponse<List<EmployeeLeaveBalanceDto>>.Fail(_localizer["Generic_UnexpectedError"]);
            }
        }

       
        public async Task InitializeAnnualLeaveForEmployeeAsync(Guid employeeId, DateTime employmentStartDate)
        {
            try
            {
                if (employeeId == Guid.Empty)
                {
                    _logger.LogWarning("InitializeAnnualLeave: EmployeeId is empty");
                    return;
                }

                var exists = await _leaveBalanceRepository
                    .Query(asNoTracking: true)
                    .AnyAsync(b => b.EmployeeId == employeeId && b.LeaveType == LeaveType.Annual);

                if (exists)
                {
                    _logger.LogInformation($"InitializeAnnualLeave: Balance already exists for employee {employeeId}");
                    return;
                }

                var balance = new EmployeeLeaveBalance
                {
                    EmployeeId = employeeId,
                    LeaveType = LeaveType.Annual,
                    BalanceDays = 0m, //start zero (add by job)
                    LastUpdatedAt = employmentStartDate,
                    CreatedAt = DateTime.Now,
                    CreatedBy = Guid.Empty
                };

                await _leaveBalanceRepository.AddAsync(balance);
                await _leaveBalanceRepository.SaveChangesAsync();

                _logger.LogInformation($"InitializeAnnualLeave: Created annual leave balance for employee {employeeId} " +
                    $"starting from {employmentStartDate}");
           
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"InitializeAnnualLeave: Unexpected error for employee {employeeId}");
            }
        }

    
        public async Task<bool> TryConsumeLeaveAsync(Guid employeeId, LeaveType leaveType, decimal days)
        {
            try
            {
                if (employeeId == Guid.Empty || days <= 0)
                {
                    _logger.LogWarning($"TryConsumeLeave: Invalid parameters. EmployeeId={employeeId}, Days={days}");
                    return false;
                }

                var balance = await _leaveBalanceRepository
                    .Query() // tracking
                    .FirstOrDefaultAsync(b =>
                        b.EmployeeId == employeeId &&
                        b.LeaveType == leaveType);

                if (balance is null)
                {
                    _logger.LogWarning($"TryConsumeLeave: No balance record for employee {employeeId}, type {leaveType}");
                    return false;
                }

                if (balance.BalanceDays < days)
                {
                    _logger.LogWarning(
                        "TryConsumeLeave: Insufficient balance for employee {EmployeeId}, type {LeaveType}. Current {Current}, Required {Required}",
                        employeeId, leaveType, balance.BalanceDays, days);

                    return false;
                }

                balance.BalanceDays -= days;
                balance.LastUpdatedAt = DateTime.Now;

                await _leaveBalanceRepository.SaveChangesAsync();

                _logger.LogInformation(
                    "TryConsumeLeave: Consumed {Days} days from employee {EmployeeId} for type {LeaveType}. New balance {Balance}",
                    days, employeeId, leaveType, balance.BalanceDays);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "TryConsumeLeave: Unexpected error for employee {EmployeeId}, type {LeaveType}",
                    employeeId, leaveType);

                return false;
            }
        }


        public async Task RefundLeaveAsync(Guid employeeId, LeaveType leaveType, decimal days)
        {
            try
            { //validation at date 
                if (employeeId == Guid.Empty || days <= 0)
                {
                    _logger.LogWarning(
                        "RefundLeave: Invalid parameters. EmployeeId={EmployeeId}, Days={Days}",
                        employeeId, days);
                    return;
                }

                var balance = await _leaveBalanceRepository
                    .Query() // tracking
                    .FirstOrDefaultAsync(b =>
                        b.EmployeeId == employeeId &&
                        b.LeaveType == leaveType);

                if (balance is null)
                {
                    _logger.LogWarning(
                        "RefundLeave: No balance record for employee {EmployeeId}, type {LeaveType}",
                        employeeId, leaveType);
                    return;
                }

                balance.BalanceDays += days;
                balance.LastUpdatedAt = DateTime.Now;

                await _leaveBalanceRepository.SaveChangesAsync();

                _logger.LogInformation(
                    "RefundLeave: Refunded {Days} days to employee {EmployeeId} for type {LeaveType}. New balance {Balance}",
                    days, employeeId, leaveType, balance.BalanceDays);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "RefundLeave: Unexpected error for employee {EmployeeId}, type {LeaveType}",
                    employeeId, leaveType);
            }
        }
    }
}
    

