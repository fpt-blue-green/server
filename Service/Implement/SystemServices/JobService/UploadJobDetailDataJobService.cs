using BusinessObjects;
using BusinessObjects.Models;
using Newtonsoft.Json;
using Quartz;
using Repositories;
using Serilog;

namespace Service
{
    public class UploadJobDetailDataJobService : IJob
    {
        private static readonly IJobRepository _jobRepository = new JobRepository();
        private static ILogger _loggerService = new LoggerService().GetDbLogger();
        private static IEmailService _emailService = new EmailService();
        private static ConfigManager _configManager = new ConfigManager();
        private static EmailTemplate _emailTempalte = new EmailTemplate();
        private static IJobDetailService _jobDetailService = new JobDetailService();

        public async Task Execute(IJobExecutionContext context)
        {
            string jobID = Guid.NewGuid().ToString();
            _loggerService.Information($"Job {jobID}: JobDetails data upload start.");
            var result = new JobResult();

            try
            {
                // Gọi Service Update data
                result = await UpdateJobDetailslInfor(jobID);

                if (result.Failure > 0)
                    throw new Exception();
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Job {jobID}: Job failed after retries. Exception: {ex.ToString()}");

                // Gửi thông báo qua email cho các admin.
                var body = _emailTempalte.uploadDataErrorTemplate.Replace("{projectName}", _configManager.ProjectName)
                                                                 .Replace("{timeHappend}", DateTime.UtcNow.AddHours(7).ToString() + " UTC+07")
                                                                 .Replace("{logLink}", _configManager.LogLink)
                                                                 .Replace("{keyWord}", jobID);
                await _emailService.SendEmail(_configManager.AdminEmails, "Thông Báo Lỗi JobDetails", body);
            }
            _loggerService.Information($"Job {jobID}: JobDetails data upload Finished. Result: {JsonConvert.SerializeObject(result)}");
        }

        public static async Task<JobResult> UpdateJobDetailslInfor(string cronJobId)
        {
            const int maxRetryAttempts = 3; // Số lần retry tối đa
            const int batchSize = 10; // Kích thước lô
            var jobResult = new JobResult();

            try
            {
                // Lấy toàn bộ danh sách Job và các Offer liên quan
                var jobs = await _jobRepository.GetAllJobInProgress();
                if (jobs.Any())
                {
                    // Chia jobs thành các lô (batch) để xử lý
                    var batches = jobs
                        .Select((job, index) => new { job, index })
                        .GroupBy(x => x.index / batchSize)
                        .Select(g => g.Select(x => x.job).ToList())
                        .ToList();

                    foreach (var batch in batches)
                    {
                        await ProcessBatchWithRetries(batch, cronJobId, maxRetryAttempts, jobResult);
                    }
                }

                jobResult.Total = jobResult.Success + jobResult.Failure;
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Job {cronJobId}: An unexpected error occurred. Exception: {ex}");
                throw new Exception(ex.ToString());
            }

            return jobResult;
        }

        private static async Task ProcessBatchWithRetries(List<Job> batch, string jobId, int maxRetryAttempts, JobResult jobResult)
        {
            int retryAttempt = 0;
            int size = batch.Count;
            while (retryAttempt < maxRetryAttempts)
            {
                var failedJobs = new List<Job>();

                foreach (var job in batch)
                {
                    try
                    {
                        await _jobDetailService.UpdateJobDetailData(job, null);
                        // Nếu thành công, tăng biến Success
                        jobResult.Success++;
                    }
                    catch (Exception ex)
                    {
                        // Nếu thất bại, thêm JobDetails vào danh sách cần retry
                        failedJobs.Add(job);
                        _loggerService.Error($"Job {jobId}: Update JobDetails {job.Id} failed. Exception: {ex}");
                    }
                }

                // Nếu không còn JobDetails nào thất bại, kết thúc vòng lặp retry
                if (!failedJobs.Any())
                {
                    break;
                }

                retryAttempt++;
                if (retryAttempt < maxRetryAttempts)
                {
                    _loggerService.Warning($"Job {jobId}: Retry attempt {retryAttempt} for {failedJobs.Count} JobDetailss. JobDetails Data: {string.Join(';', failedJobs.Select(c => c.Id))}");
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    batch = failedJobs;
                }
                else
                {
                    jobResult.Failure = size - jobResult.Success;
                    _loggerService.Error($"Job {jobId}: Max retry attempts reached for batch. {failedJobs.Count} JobDetailss failed.");
                }
            }
        }
    }
}
