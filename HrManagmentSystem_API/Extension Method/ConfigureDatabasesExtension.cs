using HrMangmentSystem_Application.Interfaces.Auth;
using HrMangmentSystem_Application.Interfaces.Repositories;
using HrMangmentSystem_Domain.Entities.Employees;
using HrMangmentSystem_Domain.Entities.Roles;
using HrMangmentSystem_Infrastructure.Implementations.Repositories;
using HrMangmentSystem_Infrastructure.Implementations.Security;
using HrMangmentSystem_Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HrMangmentSystem_API.Extension_Method
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

            services.AddScoped<IGenericRepository<Department, int>, SoftDeleteRepository<Department, int>>(); // Register SoftDeleteRepository for Department entity
            services.AddScoped<IGenericRepository<EmployeeRole, int>, GenericRepository<EmployeeRole, int>>(); // 

            services.AddScoped<ITenantRepository, TenantRepository>();

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IPasswordHasher<Employee>, PasswordHasher<Employee>>();

            return services;
        }

    }
}
