using HrMangmentSystem_Application.Interfaces.Auth;
using HrMangmentSystem_Application.Interfaces.Repository;
using HrMangmentSystem_Application.Interfaces.Services;
using HrMangmentSystem_Application.Services;
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

            services.AddScoped<ICurrentTenant, CurrentTenant>();


            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUser, HttpCurrentUser>();

           

            return services;
        }
    }
}
