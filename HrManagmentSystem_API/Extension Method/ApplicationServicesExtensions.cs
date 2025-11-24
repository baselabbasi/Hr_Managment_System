using HrMangmentSystem_Application.Interfaces;
using HrMangmentSystem_Application.Services;

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
