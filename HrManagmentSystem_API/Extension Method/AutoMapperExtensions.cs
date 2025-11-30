using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HrMangmentSystem_API.Extension_Method
{
    public static class AutoMapperExtensions
    {
        public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
        {

            services.AddAutoMapper(Assembly.Load("HrMangmentSystem_Application"));   //scan all profiles in the specified assembly
            return services;
        }
    }
}
