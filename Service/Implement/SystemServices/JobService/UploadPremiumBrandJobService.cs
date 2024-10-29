using BusinessObjects;
using BusinessObjects.Models;
using Newtonsoft.Json;
using Quartz;
using Repositories;
using Serilog;

namespace Service
{
    public class UploadPremiumBrandJobService : IJob
    {
        private static readonly IBrandRepository _brandRepository = new BrandRepository();
        private static ILogger _loggerService = new LoggerService().GetDbLogger();
        private static IEmailService _emailService = new EmailService();
        private static ConfigManager _configManager = new ConfigManager();
        private static EmailTemplate _emailTempalte = new EmailTemplate();

        public async Task Execute(IJobExecutionContext context)
        {
            string jobID = Guid.NewGuid().ToString();
            _loggerService.Information($"Job {jobID}: Brand data upload start.");
            var result = new JobResult();

            try
            {
                // Gọi Service Update data
                result = await UpdateBrandInfor(jobID);

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
                await _emailService.SendEmail(_configManager.AdminEmails, "Thông Báo Lỗi Upload Brand", body);
            }
            _loggerService.Information($"Job {jobID}: Brand data upload Finished. Result: {JsonConvert.SerializeObject(result)}");
        }

        public static async Task<JobResult> UpdateBrandInfor(string jobId)
        {
            const int maxRetryAttempts = 3;
            const int batchSize = 10;
            var jobResult = new JobResult();

            try
            {
                // Lấy danh sách brand
                var brands = await _brandRepository.GetAllExpiredPremiumBrands();
                if (brands.Any())
                {
                    // Chia các brand thành từng lô (batch)
                    var batches = brands
                        .Select((brand, index) => new { brand, index })
                        .GroupBy(x => x.index / batchSize)
                        .Select(g => g.Select(x => x.brand).ToList())
                        .ToList();

                    // Xử lý từng lô
                    foreach (var batch in batches)
                    {
                        await ProcessBatchWithRetries(batch, jobId, maxRetryAttempts, jobResult);
                    }
                }

                // Cập nhật tổng số kết quả
                jobResult.Total = jobResult.Success + jobResult.Failure;
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Job {jobId}: An unexpected error occurred. Exception: {ex}");
                throw;
            }

            return jobResult;
        }


        private static async Task ProcessBatchWithRetries(List<Brand> batch, string jobId, int maxRetryAttempts, JobResult jobResult)
        {
            int retryAttempt = 0;
            int size = batch.Count;
            while (retryAttempt < maxRetryAttempts)
            {
                var failedBrand = new List<Brand>();

                foreach (var brand in batch)
                {

                    try
                    {
                        await UpdateBrandAttributes(brand);
                        jobResult.Success++;
                        _loggerService.Information($"Job {jobId}: Successfully updated brand {brand.Id}");
                    }
                    catch (Exception ex)
                    {
                        // Nếu thất bại, thêm Brand vào danh sách cần retry
                        failedBrand.Add(brand);
                        _loggerService.Error($"Job {jobId}: Update brand {brand.Id} failed. Exception: {ex}");
                    }
                }

                // Nếu không còn Brand nào thất bại, kết thúc vòng lặp retry
                if (!failedBrand.Any())
                {
                    break;
                }

                retryAttempt++;
                if (retryAttempt < maxRetryAttempts)
                {
                    _loggerService.Warning($"Job {jobId}: Retry attempt {retryAttempt} for {failedBrand.Count} Brands. Brand Data: {string.Join(';', failedBrand.Select(c => c.Id))}");
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    batch = failedBrand;
                }
                else
                {
                    jobResult.Failure = size - jobResult.Success;
                    _loggerService.Error($"Job {jobId}: Max retry attempts reached for batch. {failedBrand.Count} Brands failed.");
                }
            }
        }

        private static async Task UpdateBrandAttributes(Brand brand)
        {
            brand.IsPremium = false;
            brand.PremiumValidTo = null;
            await _brandRepository.UpdateBrand(brand);
        }

    }
}
