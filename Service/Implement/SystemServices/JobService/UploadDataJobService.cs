using BusinessObjects.Models;
using Newtonsoft.Json;
using Quartz;
using Serilog;
using Repositories;
using BusinessObjects;

namespace Service
{
    public class UploadDataJobService : IJob
    {
        private static readonly IInfluencerRepository _influRepository = new InfluencerRepository();
        private static ILogger _loggerService = new LoggerService().GetDbLogger();
        private static IEmailService _emailService = new EmailService();
        private static ConfigManager _configManager = new ConfigManager();
        private static EmailTemplate _emailTempalte = new EmailTemplate();
        private static IChannelService _channelService;

        public UploadDataJobService(IChannelService channelService)
        {
            _channelService = channelService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            string jobID = Guid.NewGuid().ToString();
            _loggerService.Information($"Job {jobID}: Channel data upload start.");
            var result = new JobResult();

            try
            {
                // Gọi Service Update data
                result = await UpdateChannelInfor(jobID);

                if (result.Failure > 0)
                    throw new Exception();
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
                throw new Exception(ex.ToString());
            }

            return jobResult;
        }

        private static async Task ProcessBatchWithRetries(List<Channel> batch, string jobId, int maxRetryAttempts, JobResult jobResult)
        {
            int retryAttempt = 0;
            int size = batch.Count;
            while (retryAttempt < maxRetryAttempts)
            {
                var failedChannels = new List<Channel>();

                foreach (var channel in batch)
                {
                    try
                    {
                        await _channelService.UpdateInfluencerChannel(channel);
                        // Nếu thành công, tăng biến Success
                        jobResult.Success++;
                    }
                    catch (Exception ex)
                    {
                        // Nếu thất bại, thêm channel vào danh sách cần retry
                        failedChannels.Add(channel);
                        _loggerService.Error($"Job {jobId}: Update Channel {channel.Id} failed. Exception: {ex}");
                    }
                }

                // Nếu không còn channel nào thất bại, kết thúc vòng lặp retry
                if (!failedChannels.Any())
                {
                    break;
                }

                retryAttempt++;
                if (retryAttempt < maxRetryAttempts)
                {
                    _loggerService.Warning($"Job {jobId}: Retry attempt {retryAttempt} for {failedChannels.Count} channels.");
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    batch = failedChannels;
                }
                else
                {
                    jobResult.Failure = size - jobResult.Success;
                    _loggerService.Error($"Job {jobId}: Max retry attempts reached for batch. {failedChannels.Count} channels failed.");
                }
            }
        }


    }
}
