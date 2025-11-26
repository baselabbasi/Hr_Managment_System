using HrManagmentSystem_Shared.Common.Resources;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace HrMangmentSystem_API.Extension_Method
{
    public static class LocalizationExtensions
    {
        public static IServiceCollection AddLocaizationResource(this IServiceCollection services)
        {

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddControllers()
                .AddDataAnnotationsLocalization()
                .AddViewLocalization();

            services.AddSingleton<IStringLocalizer<SharedResource> , StringLocalizer<SharedResource>>();
            return services;
        }
        public static IApplicationBuilder UseAddLocalization(this IApplicationBuilder app)
        {
            var supportedCulture = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("ar")
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = supportedCulture,
                SupportedUICultures = supportedCulture,
                
            });
            return app;
        }
    }
}
