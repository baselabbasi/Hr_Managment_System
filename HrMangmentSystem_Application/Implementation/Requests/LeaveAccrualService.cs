using HrManagmentSystem_Shared.Resources;
using HrMangmentSystem_Application.Interfaces.Repositories;
using HrMangmentSystem_Application.Interfaces.Requests;
using HrMangmentSystem_Domain.Entities.Employees;
using HrMangmentSystem_Domain.Enum.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace HrMangmentSystem_Application.Implementation.Requests
{
    public class LeaveAccrualService : ILeaveAccrualService
    {
        private readonly IGenericRepository<EmployeeLeaveBalance, int> _leaveBalanceRepository;
        private readonly ILogger<LeaveAccrualService> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;

        private const decimal AnnualLeaveDaysPerYear = 14;
        public LeaveAccrualService(IStringLocalizer<SharedResource> localizer, ILogger<LeaveAccrualService> logger, IGenericRepository<EmployeeLeaveBalance, int> leaveBalanceRepository)
        {
            _localizer = localizer;
            _logger = logger;
            _leaveBalanceRepository = leaveBalanceRepository;
        }

        public async Task RunDailyAccrualAsync()
        {
            try
            {
                var today = DateTime.Now.Date;

                var balances = await _leaveBalanceRepository.Query()
                    .Where(b => b.LeaveType == LeaveType.Annual)
                    .ToListAsync();


                if (!balances.Any())
                {
                    _logger.LogInformation("LeaveAccrual: {Message}", _localizer["LeaveAccrual_NoBalancesFound"]);
                    return;
                }
                var dailyRate = AnnualLeaveDaysPerYear / 365m;

                foreach (var balance in balances)
                {
                    var last = balance.LastUpdatedAt.Date;
                    var daysDiff =(today - last).Days;

                    if (daysDiff <= 0)
                        continue;
                    
                    var added = dailyRate * daysDiff;
                    balance.BalanceDays += added;
                    balance.LastUpdatedAt = today;

                    _logger.LogInformation(
                     "LeaveAccrual: Employee {EmployeeId} +{Added} days (for {DaysDiff} days). New balance {Balance}",
                     balance.EmployeeId, added, daysDiff, balance.BalanceDays);
                }
                await _leaveBalanceRepository.SaveChangesAsync();


                _logger.LogInformation( "LeaveAccrual: {Message} at {Date}",
                    _localizer["LeaveAccrual_Completed"],today);
            
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "LeaveAccrual: {Message}", _localizer["LeaveAccrual_UnexpectedError"]);
            }
        }
    }
}
