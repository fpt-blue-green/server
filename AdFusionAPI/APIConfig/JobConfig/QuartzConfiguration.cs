using AdFusionAPI.APIConfig.JobConfig;
using Quartz;
using Service;

public static class QuartzConfiguration
{
    public static void AddQuartzServices(this IServiceCollection services)
    {
        // Đăng ký các dịch vụ với Quartz.NET
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            // Định nghĩa job đầu tiên
            var jobKey1 = new JobKey("UploadChanelDataJobService");
            q.AddJob<UploadChanelDataJobService>(opts => opts.WithIdentity(jobKey1));

            // Định nghĩa trigger với Cron Expression cho job đầu tiên (00:00 mỗi ngày)
            q.AddTrigger(opts => opts
                .ForJob(jobKey1)
                .WithIdentity("UploadChanelDataJobService")
                .WithCronSchedule("0 0 0 * * ?") // Cron cho 00:00 mỗi ngày
                //.WithCronSchedule("0 * * ? * *") // Cron Expression cho 1p
            );

            // Định nghĩa job thứ hai
            var jobKey2 = new JobKey("UploadPremiumBrandJobService");
            q.AddJob<UploadPremiumBrandJobService>(opts => opts.WithIdentity(jobKey2).StoreDurably()); // Đánh dấu là durable

            // Định nghĩa job thứ ba
            var jobKey3 = new JobKey("UploadJobDetailDataJobService");
            q.AddJob<UploadJobDetailDataJobService>(opts => opts.WithIdentity(jobKey2).StoreDurably()); // Đánh dấu là durable

            // Định nghĩa job thứ tu
            var jobKey4 = new JobKey("UploadOfferDataJobService");
            q.AddJob<UploadOfferDataJobService>(opts => opts.WithIdentity(jobKey3).StoreDurably()); // Đánh dấu là durable

            // Đăng ký JobListener
            q.AddJobListener<JobCompletionListener>();
        });

        // Đăng ký HostedService để Quartz.NET chạy cùng với ứng dụng
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

    }
}
