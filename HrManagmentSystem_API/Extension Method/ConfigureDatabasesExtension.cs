using HrMangmentSystem_Domain.Entities.Employees;
using HrMangmentSystem_Infrastructure.Models;
using HrMangmentSystem_Infrastructure.Repositories.Implementations;
using HrMangmentSystem_Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HrManagmentSystem_API.Extension_Method
{
    public static class ConfigureDatabasesExtension
    {
        public static IServiceCollection AddConfigureDatabases(this IServiceCollection services, IConfiguration configuration)
        {
            
            // Configure database contexts here
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))); 

            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>)); // Register the generic repository


            services.AddScoped<IGenericRepository<Employee, Guid>, SoftDeleteRepository<Employee, Guid>>(); // Register SoftDeleteRepository for Employee entity


            return services;
        }

    }
}
