using HrMangmentSystem_Application.Employees.Interfaces;
using HrMangmentSystem_Application.Employees.Services;

namespace HrManagmentSystem_API.Extension_Method
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            // Add application services here
            services.AddScoped<IEmployeeService, EmployeeService>();


            return services;
        }
    }
}
