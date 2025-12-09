using HrManagmentSystem_API.Interfaces;
using HrMangmentSystem_Application.Implementation.Auth;
using HrMangmentSystem_Application.Implementation.Repository;
using HrMangmentSystem_Application.Implementation.Requests;
using HrMangmentSystem_Application.Implementation.Services;
using HrMangmentSystem_Application.Interfaces.Auth;
using HrMangmentSystem_Application.Interfaces.Repository;
using HrMangmentSystem_Application.Interfaces.Requests;
using HrMangmentSystem_Application.Interfaces.Services;
using HrMangmentSystem_Infrastructure.Implementations.Identity;

namespace HrMangmentSystem_API.Extension_Method
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            // Add application services here
            services.AddScoped<IEmployeeService, EmployeeService>();

            services.AddScoped<IDepartmentService, DepartmentService>();

            services.AddScoped<IEmployeeRoleService, EmployeeRoleService>();
            
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IJobPositionService , JobPositionService>();

            services.AddScoped<ICurrentTenant, CurrentTenant>();


            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUser, HttpCurrentUser>();

            services.AddScoped<IJobApplicationService, JobApplicationService>();
            services.AddScoped<IDocumentCvService, DocumentCvService>();

            services.AddScoped<IFileStorageService, FileStorageService>();
           
            services.AddScoped<ILeaveRequestService, LeaveRequestService>();
            services.AddScoped<IEmployeeDataChangeRequestService, EmployeeDataChangeRequestService>();
            services.AddScoped<IResignationRequestService, ResignationRequestService>();
            services.AddScoped<IFinancialRequestService, FinancialRequestService>();
            services.AddScoped<IRequestService, RequestService>();
            services.AddScoped<ILeaveBalanceService, LeaveBalanceService>();
            services.AddScoped<ILeaveAccrualService, LeaveAccrualService>();
            
            return services;
        }
    }
}
