using Quartz;
using Service.Implement.SystemServices.JobService;

public static class QuartzConfiguration
{
    public static void AddQuartzServices(this IServiceCollection services)
    {
        // Đăng ký Quartz.NET
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            // Định nghĩa job
            var jobKey = new JobKey("UploadDataJobService");
            q.AddJob<UploadDataJobService>(opts => opts.WithIdentity(jobKey));

            // Định nghĩa trigger với Cron Expression
            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("UploadDataTrigger")
                /*.WithCronSchedule("0 * * ? * *")*/ // Cron Expression cho 1p
                .WithCronSchedule("0 0 0 * * ?") // Cron Expression cho 00:00 mỗi ngày
            );
        });

        // Đăng ký HostedService để Quartz.NET chạy cùng với ứng dụng
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    }
}
