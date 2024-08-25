using BusinessObjects.Models;
using BusinessObjects.DTOs;
using Newtonsoft.Json;
using Quartz;
using Repositories.Implement;
using Repositories.Interface;
using Serilog;
using Service.Domain;
using Service.Interface.UtilityServices;
using Service.Resources;

namespace Service.Implement.SystemServices.JobService
{
    public class UploadDataJobService : IJob
    {
        private static readonly IInfluencerRepository _influRepository = new InfluencerRepository();
        private static ILogger _loggerService = new LoggerService().GetDbLogger();
        private static IEmailService _emailService = new EmailService();
        private static ConfigManager _configManager = new ConfigManager();
        private static EmailTemplate _emailTempalte = new EmailTemplate();

        public async Task Execute(IJobExecutionContext context)
        {
            string jobID = Guid.NewGuid().ToString();
            _loggerService.Information($"Job {jobID}: Channel data upload start.");
            var result = new JobResult();

            try
            {
                // Gọi Service Update data
                result = await UpdateChannelInfor(jobID);

            }
            catch (Exception ex)
            {
                _loggerService.Error($"Job {jobID}: Job failed after retries. Exception: {ex.Message}");

                // Gửi thông báo qua email cho các admin.
                var body = _emailTempalte.uploadDataErrorTemplate.Replace("{projectName}", _configManager.ProjectName)
                                                                 .Replace("{timeHappend}", DateTime.UtcNow.AddHours(7).ToString() + " UTC+07")
                                                                 .Replace("{logLink}", _configManager.LogLink)
                                                                 .Replace("{keyWord}", jobID);
                await _emailService.SendEmail(_configManager.AdminEmails, "Thông Báo Lỗi", body);
            }
            _loggerService.Information($"Job {jobID}: Channel data upload Finished. Result: {JsonConvert.SerializeObject(result)}");
        }

        public static async Task<JobResult> UpdateChannelInfor(string jobId)
        {
            const int maxRetryAttempts = 3; // Số lần retry tối đa
            const int batchSize = 10; // Kích thước lô
            var jobResult = new JobResult();

            try
            {
                var influencers = await _influRepository.GetAlls();
                if (influencers.Any())
                {
                    var allChannels = influencers.SelectMany(influencer => influencer.Channels).ToList();
                    var batches = allChannels
                        .Select((channel, index) => new { channel, index })
                        .GroupBy(x => x.index / batchSize)
                        .Select(g => g.Select(x => x.channel).ToList())
                        .ToList();

                    foreach (var batch in batches)
                    {
                        await ProcessBatchWithRetries(batch, jobId, maxRetryAttempts, jobResult);
                    }
                }

                jobResult.Total = jobResult.Success + jobResult.Failure;
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Job {jobId}: An unexpected error occurred. Exception: {ex}");
            }

            return jobResult;
        }

        private static async Task ProcessBatchWithRetries(List<Channel> batch, string jobId, int maxRetryAttempts, JobResult jobResult)
        {
            int retryAttempt = 0;

            while (retryAttempt < maxRetryAttempts)
            {
                var failedChannels = new List<Channel>();

                foreach (var channel in batch)
                {
                    try
                    {
                        //Sẽ có 1 cái hàm nhận channel. Sau đó nó sẽ lấy url ra và gọi service lấy thông tin. Sau đó update lại thông tin của Channel đó
                        // Nếu thành công, tăng biến Success
                        jobResult.Success++;
                    }
                    catch (Exception ex)
                    {
                        // Nếu thất bại, tăng biến Failure và thêm channel vào danh sách cần retry
                        jobResult.Failure++;
                        failedChannels.Add(channel);
                        _loggerService.Error($"Job {jobId}: Update Channel {channel.Id} failed. Exception: {ex}");
                    }
                }

                if (!failedChannels.Any())
                {
                    break; // Nếu không có channel nào thất bại, kết thúc vòng lặp retry
                }

                retryAttempt++;
                if (retryAttempt < maxRetryAttempts)
                {
                    _loggerService.Warning($"Job {jobId}: Retry attempt {retryAttempt} for {failedChannels.Count} channels.");
                    await Task.Delay(TimeSpan.FromSeconds(5)); // Chờ trước khi retry lần tiếp theo
                    batch = failedChannels; // Retry các channel bị lỗi
                }
                else
                {
                    _loggerService.Error($"Job {jobId}: Max retry attempts reached for batch. {failedChannels.Count} channels failed.");
                }
            }
        }

    }
}
