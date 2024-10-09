using Quartz;

namespace AdFusionAPI.APIConfig.JobConfig
{
    public class JobCompletionListener : IJobListener
    {
        public string Name => "JobCompletionListener";

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            // Kiểm tra nếu job 1 đã hoàn thành
            if (context.JobDetail.Key.Name == "UploadChanelDataJobService")
            {
                var scheduler = context.Scheduler;
                // Trigger job thứ hai
                var job2Key = new JobKey("UploadJobDetailDataJobService");
                await scheduler.TriggerJob(job2Key);

                // Lấy ngày hiện tại
                var currentDate = DateTime.UtcNow;

                // Kiểm tra xem hôm nay có phải là ngày chạy của job 3 không
                if (currentDate.Day % 2 == 0) // Chạy mỗi 2 ngày một lần
                {
                    var job3Key = new JobKey("UploadOfferDataJobService");

                    // Trigger job 3
                    await scheduler.TriggerJob(job3Key);
                }
            }
        }
    }
}
