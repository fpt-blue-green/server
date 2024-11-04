using BusinessObjects;
using BusinessObjects.Models;
using Newtonsoft.Json;
using Quartz;
using Repositories;
using Serilog;
using static BusinessObjects.JobEnumContainer;

namespace Service
{
    public class UploadOfferDataJobService : IJob
    {
        private static ILogger _loggerService = new LoggerService().GetDbLogger();
        private static IEmailService _emailService = new EmailService();
        private static ConfigManager _configManager = new ConfigManager();
        private static EmailTemplate _emailTemplate = new EmailTemplate();
        private static IJobRepository _jobRepository = new JobRepository();

        public async Task Execute(IJobExecutionContext context)
        {
            string jobID = Guid.NewGuid().ToString();
            _loggerService.Information($"Job {jobID}: Offer data update start.");
            var result = new JobResult();

            try
            {
                // Gọi Service Update data
                result = await UpdateJobAndOfferData(jobID);

                if (result.Failure > 0)
                    throw new Exception($"Job {jobID}: Có {result.Failure} offer bị lỗi khi cập nhật.");
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Job {jobID}: Job failed after retries. Exception: {ex}");

                // Gửi thông báo qua email cho các admin.
                var body = _emailTemplate.uploadDataErrorTemplate
                                        .Replace("{projectName}", _configManager.ProjectName)
                                        .Replace("{timeHappend}", DateTime.Now.ToString() + " UTC+07")
                                        .Replace("{logLink}", _configManager.LogLink)
                                        .Replace("{keyWord}", jobID);
                await _emailService.SendEmail(_configManager.AdminEmails, "Thông Báo Lỗi Cập Nhật Offer", body);
            }

            _loggerService.Information($"Job {jobID}: Offer data update finished. Result: {JsonConvert.SerializeObject(result)}");
        }

        private static async Task<JobResult> UpdateJobAndOfferData(string jobId)
        {
            const int maxRetryAttempts = 3; // Số lần retry tối đa
            const int batchSize = 10; // Kích thước lô
            var jobResult = new JobResult();

            try
            {
                // Lấy toàn bộ danh sách Job và các Offer liên quan
                var jobs = await _jobRepository.GetAllPedingJob();
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

        private static async Task ProcessBatchWithRetries(List<Job> batch, string jobId, int maxRetryAttempts, JobResult jobResult)
        {
            int retryAttempt = 0;
            int size = batch.Count;

            while (retryAttempt < maxRetryAttempts)
            {
                var failedOffers = new List<Offer>();

                foreach (var job in batch)
                {
                    // Chỉ lấy những offer có status là Offering hoặc WaitingPayment và CreatedDate từ 10 ngày trước
                    var eligibleOffers = job.Offers.Where(o =>
                        (o.Status == (int)EOfferStatus.Offering || o.Status == (int)EOfferStatus.WaitingPayment) && o.CreatedAt <= DateTime.Now.AddDays(-10)).ToList();

                    try
                    {
                        foreach (var offer in eligibleOffers)
                        {
                            try
                            {
                                // Cập nhật trạng thái cho từng Offer
                                offer.Status = (int)EOfferStatus.Expired;
                                _loggerService.Information($"Job {jobId}: Offer {offer.Id} status updated to Expired.");
                            }
                            catch (Exception ex)
                            {
                                // Nếu thất bại, thêm offer vào danh sách cần retry
                                failedOffers.Add(offer);
                                _loggerService.Error($"Job {jobId}: Update Offer {offer.Id} failed. Exception: {ex}");
                                continue; // Bỏ qua offer này và tiếp tục với offer tiếp theo
                            }
                        }

                        // Cập nhật Job cùng các Offer của nó nếu không có offer nào thất bại
                        if (!failedOffers.Any())
                        {
                            job.Status = (int)EJobStatus.NotCreated;
                            await _jobRepository.UpdateJobAndOffer(job);

                            // Nếu cập nhật thành công, tăng biến Success
                            jobResult.Success++;
                        }
                        else
                        {
                            _loggerService.Warning($"Job {jobId}: {failedOffers.Count} offers failed to update.");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Xử lý lỗi chung ở đây
                        _loggerService.Error($"Job {jobId}: An error occurred while processing offers. Exception: {ex}");
                    }

                }

                // Nếu không còn offer nào thất bại, kết thúc vòng lặp retry
                if (!failedOffers.Any())
                {
                    break;
                }

                retryAttempt++;
                if (retryAttempt < maxRetryAttempts)
                {
                    _loggerService.Warning($"Job {jobId}: Retry attempt {retryAttempt} for {failedOffers.Count} offers. Offer IDs: {string.Join(';', failedOffers.Select(o => o.Id))}");
                    await Task.Delay(TimeSpan.FromSeconds(3));

                    // Cập nhật batch chỉ còn các offer thất bại
                    foreach (var job in batch)
                    {
                        job.Offers = failedOffers.Where(offer => job.Offers.Any(o => o.Id == offer.Id)).ToList();
                    }
                }
                else
                {
                    jobResult.Failure += failedOffers.Count;
                    _loggerService.Error($"Job {jobId}: Max retry attempts reached for batch. {failedOffers.Count} offers failed.");
                }
            }
        }

    }
}
