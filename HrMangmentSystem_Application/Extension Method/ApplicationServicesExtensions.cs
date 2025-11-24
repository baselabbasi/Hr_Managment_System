using HrMangmentSystem_Application.Interfaces;
using HrMangmentSystem_Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HrMangmentSystem_Application.Extension_Method
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            // Add application services here
            services.AddScoped<IEmployeeService, EmployeeService>();

            services.AddScoped<IDepartmentService, DepartmentService>();


            return services;
        }
    }
}
