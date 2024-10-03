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
                // Lấy ngày hiện tại
                var currentDate = DateTime.UtcNow;

                // Kiểm tra xem hôm nay có phải là ngày chạy của job 2 không
                if (currentDate.Day % 4 == 0) // Chạy mỗi 4 ngày một lần
                {
                    var scheduler = context.Scheduler;
                    var job2Key = new JobKey("UploadOfferDataJobService");

                    // Trigger job 2
                    await scheduler.TriggerJob(job2Key);
                }
            }
        }
    }
}
