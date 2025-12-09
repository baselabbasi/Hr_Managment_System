using HrMangmentSystem_Infrastructure.Job;
using Quartz;



namespace HrManagmentSystem_API.Extension_Method
{
    public static class QuartzExtensions
    {
        public static IServiceCollection AddLeaveAccrualQuartz(this IServiceCollection services)
        {
            services.AddQuartz(q =>
            {
                var jobKey = new JobKey("LeaveAccrualJob");

                q.AddJob<LeaveAccrualJob>(opts => opts.WithIdentity(jobKey));

                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("LeaveAccrualJob-trigger")
                    .WithCronSchedule("0 0 2 * * ?"));
              
            });

            services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
            });

            return services;
        }
    }
}
