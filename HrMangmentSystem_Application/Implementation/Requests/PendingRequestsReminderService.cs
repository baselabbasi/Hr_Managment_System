using HrManagmentSystem_Shared.Enum.Request;
using HrManagmentSystem_Shared.Resources;
using HrMangmentSystem_Application.Interfaces.Notifications;
using HrMangmentSystem_Application.Interfaces.Requests;
using HrMangmentSystem_Domain.Constants;
using HrMangmentSystem_Domain.Entities.Employees;
using HrMangmentSystem_Domain.Entities.Requests;
using HrMangmentSystem_Domain.Entities.Roles;
using HrMangmentSystem_Infrastructure.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Text;

namespace HrMangmentSystem_Application.Implementation.Requests
{
    public class PendingRequestsReminderService : IPendingRequestsReminderService
    {
        private readonly IGenericRepository<GenericRequest, int> _genericRequestRepository;
        private readonly IGenericRepository<Employee , Guid> _employeeRepository;
        private readonly IGenericRepository<EmployeeRole , int> _employeeRoleRepository;
        private readonly IGenericRepository<Role , int> _roleRepository;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<PendingRequestsReminderService> _logger;
       

        public PendingRequestsReminderService(
            IGenericRepository<GenericRequest, int> genericRequestRepository,
            IGenericRepository<Employee, Guid> employeeRepository,
            IGenericRepository<EmployeeRole, int> employeeRoleRepository,
            IGenericRepository<Role, int> roleRepository,
            IEmailSender emailSender,
            ILogger<PendingRequestsReminderService> logger)
        {
            _genericRequestRepository = genericRequestRepository;
            _employeeRepository = employeeRepository;
            _employeeRoleRepository = employeeRoleRepository;
            _roleRepository = roleRepository;
            _emailSender = emailSender;
            _logger = logger;
           
        }

        public async Task SendPendingRequestsSummaryAsync()
        {
            try
            {       //all requests with status Submitted or InReview
                var pendingRequests = await _genericRequestRepository
                 .Query(asNoTracking: true)
                 .Include(g => g.RequestedByEmployee)
                 .Where(g =>
                     g.RequestStatus == RequestStatus.Submitted ||
                     g.RequestStatus == RequestStatus.InReview)
                 .OrderBy(g => g.TenantId)
                 .ThenBy(g => g.RequestedAt)
                 .ToListAsync();
                // If no pending requests, log and exit
                if (!pendingRequests.Any())
                {
                    _logger.LogInformation("PendingRequestsReminder: No pending requests found.");
                    return;
                }
                // Get HR Admin Role Id
                var hrAdminRoleId = await _roleRepository
                   .Query(asNoTracking: true)
                   .Where(r => r.Name == RoleNames.HrAdmin)
                   .Select(r => r.Id)
                   .FirstOrDefaultAsync();

                if (hrAdminRoleId == 0)
                {
                    _logger.LogWarning("PendingRequestsReminder: HrAdmin role not found.");
                    return;
                }
                // Group pending requests by TenantId
                var groupedByTenant = pendingRequests
                   .GroupBy(r => r.TenantId)
                   .ToList();

                foreach (var tenantGroup in groupedByTenant)
                {
                    var tenantId = tenantGroup.Key;

                    // Get HR Admin emails for the tenant
                    var hrAdminEmails = await _employeeRoleRepository
                        .Query(asNoTracking: true)
                        .Include(er => er.Employee)
                        .Where(er =>
                            er.TenantId == tenantId &&
                            er.RoleId == hrAdminRoleId &&
                            er.Employee != null &&
                            !string.IsNullOrEmpty(er.Employee.Email))
                        .Select(er => er.Employee!.Email)
                        .Distinct()
                        .ToListAsync();

                    if (!hrAdminEmails.Any())
                    {
                        _logger.LogInformation(
                            "PendingRequestsReminder: No HR Admins found for tenant {TenantId}. Skipping.",
                            tenantId);
                        continue;
                    }
                    // Build email content
                    var sb = new StringBuilder();

                    sb.AppendLine("Hello,");
                    sb.AppendLine();
                    sb.AppendLine("Here is a list of pending requests that require your review/approval for your company:");
                    sb.AppendLine();

                    // List each pending request
                    foreach (var req in tenantGroup)
                    {
                        var employeeName = req.RequestedByEmployee != null
                            ? $"{req.RequestedByEmployee.FirstName} {req.RequestedByEmployee.LastName}"
                            : "Unknown";

                        sb.AppendLine(
                           $"- Request Id: {req.Id}, " +
                           $"Type: {req.RequestType}, " +
                           $"Status: {req.RequestStatus}, " +
                           $"Employee: {employeeName}, " +
                           $"Created At: {req.RequestedAt:yyyy-MM-dd HH:mm}");
                    }

                    sb.AppendLine();
                    sb.AppendLine("Best regards,");
                    sb.AppendLine("HR Management System");

                    var subject = "Reminder: Pending requests require your approval";

                    foreach (var email in hrAdminEmails)
                    {
                        await _emailSender.SendEmailAsync(email, subject, sb.ToString());
                    }

                    _logger.LogInformation(
                        "PendingRequestsReminder: Sent pending requests email for tenant {TenantId} to {Count} HR admins.",
                        tenantId, hrAdminEmails.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending pending requests summary.");
                throw;
            }
        }
    }
}
