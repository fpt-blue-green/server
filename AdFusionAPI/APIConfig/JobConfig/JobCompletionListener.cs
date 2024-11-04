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
                var job2Key = new JobKey("UploadPremiumBrandJobService");
                await scheduler.TriggerJob(job2Key);

                // Trigger job thứ ba
                var job3Key = new JobKey("UploadJobDetailDataJobService");
                await scheduler.TriggerJob(job3Key);

                // Trigger job thứ tư
                var job4Key = new JobKey("UploadOfferDataJobService");
                await scheduler.TriggerJob(job4Key);

                // Trigger job thứ năm
                var job5Key = new JobKey("UploadJobPaymentJobService");
                await scheduler.TriggerJob(job5Key);
            }
        }
    }
}
