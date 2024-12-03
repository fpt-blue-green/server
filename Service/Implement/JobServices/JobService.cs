using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Quartz;
using Repositorie;
using Repositories;
using Serilog;
using System.Transactions;
using static BusinessObjects.AuthEnumContainer;
using static BusinessObjects.JobEnumContainer;

namespace Service
{
    public class JobService : IJobService
    {
        private static IJobRepository _jobRepository = new JobRepository();
        private static IJobDetailRepository _jobDetailRepository = new JobDetailRepository();
        private static IJobDetailService _jobDetailService = new JobDetailService();
        private static readonly EmailTemplate _emailTemplate = new EmailTemplate();
        private static readonly IEmailService _emailService = new EmailService();
        private static readonly ConfigManager _configManager = new ConfigManager();
        private static IUserRepository _userRepository = new UserRepository();
        private static IPaymentRepository _paymentBookingRepository = new PaymentRepository();
        private static ILogger _loggerService = new LoggerService().GetDbLogger();

        private static readonly IMapper _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        }).CreateMapper();

        public async Task CreateJob(JobRequestDTO job)
        {
            var jobnew = _mapper.Map<Job>(job);
            await _jobRepository.Create(jobnew);
        }

        public async Task<FilterListResponse<JobDTO>> GetAllJobByCurrentAccount(UserDTO user, JobFilterDTO filter)
        {
            IEnumerable<Job> jobs = Enumerable.Empty<Job>();

            if (user.Role == ERole.Influencer)
            {
                jobs = await _jobRepository.GetJobInfluencerByUserId(user.Id);
            }
            else
            {
                jobs = await _jobRepository.GetJobBrandByUserId(user.Id);
            }

            #region filter
            if (filter.CampaignId != null)
            {
                jobs = jobs.Where(f => f.CampaignId == filter.CampaignId);
            }

            if (filter.From.HasValue)
            {
                jobs = jobs.Where(job =>
                    job.Offers
                        .OrderByDescending(o => o.CreatedAt)
                        .FirstOrDefault()?.From == (int)filter.From
                );
            }

            if (filter.JobStatuses != null && filter.JobStatuses.Any())
            {
                jobs = jobs.Where(i => filter.JobStatuses.Contains((EJobStatus)i.Status));
            }

            if (filter.CampaignStatuses != null && filter.CampaignStatuses.Any())
            {
                jobs = jobs.Where(i => filter.CampaignStatuses.Contains((ECampaignStatus)i.Campaign.Status));
            }
            #endregion

            int totalCount = jobs.Count();

            #region paging
            int pageSize = filter.PageSize;
            jobs = jobs
                .Skip((filter.PageIndex - 1) * pageSize)
                .Take(pageSize);
            #endregion

            // Map từng job và chọn offer có CreateAt mới nhất thỏa mãn điều kiện filter.From
            var jobDTOs = jobs.Select(job =>
            {
                // Lấy offer mới nhất cho mỗi job (không cần kiểm tra điều kiện From vì đã lọc ở trên)
                var latestOffer = job.Offers
                    .OrderByDescending(offer => offer.CreatedAt)
                    .FirstOrDefault();

                var jobDTO = _mapper.Map<JobDTO>(job);

                // Chỉ ánh xạ offer mới nhất vào JobDTO
                if (jobDTO != null && latestOffer != null)
                {
                    jobDTO.Offer = _mapper.Map<OfferDTO>(latestOffer);
                }

                return jobDTO;
            });

            return new FilterListResponse<JobDTO>
            {
                TotalCount = totalCount,
                Items = jobDTOs
            };
        }

        public async Task<List<JobStatistical>> Statistical(UserDTO user)
        {

            IEnumerable<Job> jobs = Enumerable.Empty<Job>();

            if (user.Role == ERole.Influencer)
            {
                jobs = await _jobRepository.GetJobInfluencerByUserId(user.Id);
            }
            else
            {
                jobs = await _jobRepository.GetJobBrandByUserId(user.Id);
            }

            var result = Enum.GetValues(typeof(EJobStatus))
                     .Cast<EJobStatus>()
                     .Select(status => new JobStatistical
                     {
                         JobStatus = status,
                         Count = 0
                     }).ToList();

            var jobCounts = jobs.GroupBy(j => (EJobStatus)j.Status)
                                   .Select(g => new JobStatistical
                                   {
                                       JobStatus = g.Key,
                                       Count = g.Count()
                                   });

            foreach (var jobCount in jobCounts)
            {
                var item = result.First(r => r.JobStatus == jobCount.JobStatus);
                item.Count = jobCount.Count;
            }

            return result;
        }

        public async Task BrandPaymentJob(Guid jobId, UserDTO userDto)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var job = await _jobRepository.GetJobFullDetailById(jobId);

                    var offer = job.Offers.FirstOrDefault(f => f.Status == (int)EOfferStatus.WaitingPayment);
                    if (offer == null)
                    {
                        throw new InvalidOperationException("Không có đề nghị nào được chấp thuận.");
                    }

                    var user = job.Campaign.Brand.User;

                    if (user.Wallet < offer.Price)
                    {
                        throw new InvalidOperationException("Không đủ tiền để thanh toán. Vui lòng đến trang nạp tiền để hoàn thành đề nghị.");
                    }

                    user.Wallet -= offer.Price;
                    await _userRepository.UpdateUser(user);
                    if (job.Campaign.Status == (int)ECampaignStatus.Active)
                    {
                        job.Status = (int)EJobStatus.InProgress;
                    }
                    else
                    {
                        job.Status = (int)EJobStatus.Approved;
                    }

                    job.Offers.FirstOrDefault(f => f.Status == (int)EOfferStatus.WaitingPayment)!.Status = (int)EOfferStatus.Done;
                    await _jobRepository.UpdateJobAndOffer(job);

                    //Save Payment data.
                    await _paymentBookingRepository.CreatePaymentBooking(new PaymentBooking
                    {
                        JobId = jobId,
                        Amount = offer.Price,
                        Type = (int)EPaymentType.BrandPayment,
                    });

                    var influencerUser = job.Influencer.User;
                    scope.Complete();

                    //send mail
                    await SendMail(job, offer, "được thanh toán", "được thanh toán", "Bạn có thể bắt đầu công việc ngay khi chiến dịch được khởi động.");
                }
                catch
                {
                    throw;

                }
            }
        }

        public async Task BrandCancelJob(Guid jobId, UserDTO userDto)
        {
            var job = await _jobRepository.GetJobFullDetailById(jobId);

            var offer = job.Offers.FirstOrDefault(f => f.Status == (int)EOfferStatus.WaitingPayment);
            if (offer == null)
            {
                throw new InvalidOperationException("Không có đề nghị nào được chấp thuận.");
            }
            var user = job.Campaign.Brand.User;
            if (userDto.Id != user.Id)
            {
                throw new AccessViolationException($"Nhãn hàng với Id {user.Id} đang từ chối thanh toán có Id bị bất thường {userDto.Id}");
            }

            job.Status = (int)EJobStatus.NotCreated;
            job.Offers.FirstOrDefault(f => f.Status == (int)EOfferStatus.WaitingPayment)!.Status = (int)EOfferStatus.Cancelled;
            await _jobRepository.UpdateJobAndOffer(job);

            //send mail
            await SendMail(job, offer, "bị từ chối thanh toán", "bị từ chối", "Rất tiếc, thanh toán cho chiến dịch đã bị hủy, do đó bạn không thể bắt đầu công việc.");
        }

        public async Task BrandCompleteJob(Guid jobId, UserDTO userDto)
        {
            var job = await _jobRepository.GetJobFullDetailById(jobId);

            var user = job.Campaign.Brand.User;
            if (userDto.Id != user.Id)
            {
                throw new AccessViolationException($"Nhãn hàng với Id {user.Id} đang đánh giấu hoàn thành Công việc có Id bị bất thường {userDto.Id}");
            }

            if (job.Status != (int)EJobStatus.InProgress)
            {
                throw new InvalidOperationException("Chỉ những công việc đang triển khai mới có thể thực hiện hành động này");
            }
            if (job.Campaign.Status != (int)ECampaignStatus.Active)
            {
                throw new InvalidOperationException("Chỉ có Chiến dịch đang triển khai mới có thể thay đổi trang thai công việc.");
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var isPayment = await _paymentBookingRepository.GetInfluencerPaymentByJobId(jobId) != null;
                    if (isPayment == false)
                    {
                        var offer = job.Offers.FirstOrDefault(o => o.Status == (int)EOfferStatus.Done);
                        var userGet = await _userRepository.GetUserByInfluencerId(job.InfluencerId) ?? throw new KeyNotFoundException();
                        userGet.Wallet += offer!.Price;
                        await _userRepository.UpdateUser(userGet);

                        var paymentBooking = new PaymentBooking
                        {
                            Amount = offer!.Price,
                            JobId = job.Id,
                            PaymentDate = DateTime.Now,
                            Type = (int)EPaymentType.InfluencerPayment,
                        };
                        await _paymentBookingRepository.CreatePaymentBooking(paymentBooking);
                    }
                    job.Status = (int)EJobStatus.Completed;
                    await _jobRepository.UpdateJobAndOffer(job);

                    scope.Complete();

                }
                catch
                {
                    throw;
                }
            }

            //send mail
            var resultMessage = "Lưu ý: Khoản tiền công của bạn sẽ được chuyển khoản sau 3 ngày làm việc, trừ khi có phản hồi từ phía Nhãn hàng trong thời gian này. Điều này nhằm đảm bảo rằng mọi công việc và thanh toán đều được xử lý nhanh chóng và minh bạch. Chúng tôi rất mong nhận được sự hợp tác của bạn và luôn sẵn sàng hỗ trợ nếu bạn có bất kỳ thắc mắc nào liên quan đến quá trình thanh toán.";
            await SendMailStatus(job, "Đã hoàn thành", resultMessage);
        }

        public async Task BrandFaliedJob(Guid jobId, UserDTO userDto)
        {
            var job = await _jobRepository.GetJobFullDetailById(jobId);

            var user = job.Campaign.Brand.User;
            if (userDto.Id != user.Id)
            {
                throw new AccessViolationException($"Nhãn hàng với Id {user.Id} đang đang đánh giấu Công việc thất bại có Id bị bất thường {userDto.Id}");
            }
            if (job.Status != (int)EJobStatus.InProgress)
            {
                throw new InvalidOperationException("Chỉ những công việc đang triển khai mới có thể thực hiện hành động này");
            }
            if (job.Campaign.Status != (int)ECampaignStatus.Active)
            {
                throw new InvalidOperationException("Chỉ có Chiến dịch đang triển khai mới có thể thay đổi trang thai công việc.");
            }
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var offer = job.Offers.FirstOrDefault(o => o.Status == (int)EOfferStatus.Done);
                    user.Wallet += offer!.Price;
                    await _userRepository.UpdateUser(user);

                    var paymentBooking = new PaymentBooking
                    {
                        Amount = offer!.Price,
                        JobId = job.Id,
                        PaymentDate = DateTime.Now,
                        Type = (int)EPaymentType.Refund,
                    };
                    await _paymentBookingRepository.CreatePaymentBooking(paymentBooking);

                    job.Status = (int)EJobStatus.Failed;
                    await _jobRepository.UpdateJobAndOffer(job);

                    scope.Complete();
                }
                catch
                {
                    throw;
                }
            }
            //send mail
            var resultMessage = "Lưu ý: Công việc của bạn đã bị đánh dấu là thất bại. Tuy nhiên, bạn vẫn có 3 ngày để phản hồi với Nhãn hàng nếu có bất kỳ sai sót hoặc vấn đề nào cần được xem xét lại. Đây là cơ hội để đảm bảo rằng mọi thông tin đều chính xác và minh bạch. Nếu cần hỗ trợ thêm hoặc có câu hỏi nào, vui lòng liên hệ với chúng tôi.";
            await SendMailStatus(job, "Đã thất bại", resultMessage);
        }

        public async Task BrandReopenJob(Guid jobId, UserDTO userDto)
        {
            var job = await _jobRepository.GetJobFullDetailById(jobId);
            var offer = job.Offers.FirstOrDefault(f => f.Status == (int)EOfferStatus.Done) ?? throw new KeyNotFoundException();

            var user = job.Campaign.Brand.User;
            if (userDto.Id != user.Id)
            {
                throw new AccessViolationException($"Nhãn hàng với Id {user.Id} đang đang đánh giấu Công việc Khởi động lại có Id bị bất thường {userDto.Id}");
            }

            if (job.Campaign.Status != (int)ECampaignStatus.Active)
            {
                throw new InvalidOperationException("Chỉ có Chiến dịch đang triển khai mới có thể thay đổi trang thai công việc.");
            }

            if (job.Status != (int)EJobStatus.InProgress)
            {
                throw new InvalidOperationException("Chỉ những công việc đã thất bại mới có thể thực hiện hành động này");
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (user.Wallet < offer.Price)
                    {
                        throw new InvalidOperationException("Không đủ tiền để thanh toán. Vui lòng đến trang nạp tiền để hoàn thành đề nghị.");
                    }

                    user.Wallet -= offer.Price;
                    await _userRepository.UpdateUser(user);

                    job.Status = (int)EJobStatus.InProgress;
                    await _jobRepository.Update(job);

                    scope.Complete();
                }
                catch
                {
                    throw;
                }
            }
        }

        public async Task AttachPostLink(Guid jobId, UserDTO userDTO, JobLinkDTO linkDTO)
        {
            var job = await _jobRepository.GetJobInProgress(jobId);

            if (job == null)
            {
                throw new InvalidOperationException("Công việc này chưa bắt đầu hoặc đã kết thúc, không thể thêm liên kết");
            }

            if (job.Influencer.User.Id != userDTO.Id)
            {
                throw new AccessViolationException();
            }

            if(linkDTO.Link.GroupBy(x => x).Any(g => g.Count() > 1))
            {
                throw new InvalidOperationException("Đã phát hiện đường dẫn trùng lập. Vui lòng kiểm tra lại.");
            }

            var offer = job.Offers.FirstOrDefault(o => o.Status == (int)EOfferStatus.Done)
                        ?? throw new KeyNotFoundException();

            if (offer.Quantity < linkDTO.Link.Count)
            {
                throw new InvalidOperationException("Số lượng đường dẫn video vượt qua số lượng thỏa thuận.");
            }

            // Lấy các liên kết đã tồn tại trong JobDetails
            var existingLinks = await _jobDetailRepository.GetJobDetailsByJobId(jobId);

            // Cập nhật hoặc tạo mới các liên kết
            foreach (var newLink in linkDTO.Link)
            {
                // Kiểm tra nếu link đã tồn tại
                var existingJobDetail = existingLinks.FirstOrDefault(jd => jd.Link == newLink);
                if (existingJobDetail == null)
                {
                    // Nếu link mới không tồn tại hoặc không được phê duyệt, tạo mới hoặc cập nhật
                    await _jobDetailService.UpdateJobDetailData(job, newLink);
                }
            }

            // Xử lý các link chưa được duyệt và không nằm trong danh sách mới
            var unapprovedLinksToRemove = existingLinks
                .Where(jd => !jd.IsApprove && !linkDTO.Link.Contains(jd.Link))
                .ToList();

            foreach (var removedLink in unapprovedLinksToRemove)
            {
                await _jobDetailRepository.Delete(removedLink);
            }
        }

        public async Task ApprovePostLink(Guid jobId, string link)
        {
            var job = await _jobRepository.GetJobInProgress(jobId);

            if (job == null)
            {
                throw new InvalidOperationException("Công việc này chưa bắt đầu hoặc đã kết thúc, không thể Phê duyệt liên kết");
            }
            var jobDetail = await _jobDetailRepository.GetByLinkAndJobId(link, jobId) ?? throw new KeyNotFoundException();

            jobDetail.IsApprove = true;

            await _jobDetailRepository.Update(jobDetail);
        }

        public async Task<List<JobLinkResponseDTO>> GetJobLink(Guid jobId)
        {
            var jobDetail = await _jobDetailRepository.GetLinkByJobId(jobId);
            var jobLinkResponseList = jobDetail
                .Select(j => new JobLinkResponseDTO
                {
                    Link = j.Link!, 
                    IsApprove = j.IsApprove,
                })
                .ToList();
            return jobLinkResponseList;
        }

        public async Task SendMail(Job job, Offer offer, string title, string status, string endQuote)
        {
            try
            {
                string subject = "Thông báo trạng thái thanh toán của Offer";
                var influencerUser = job.Influencer.User;
                var brandUser = job.Campaign.Brand.User;

                var body = _emailTemplate.brandPaymentOffer
                    .Replace("{Title}", title)
                    .Replace("{InfluencerName}", influencerUser.DisplayName)
                    .Replace("{BrandName}", brandUser.DisplayName)
                    .Replace("{Status}", status)
                    .Replace("{ContentType}", ((EContentType)offer.ContentType).GetEnumDescription())
                    .Replace("{Price}", offer?.Price.ToString("N0") + " VND" ?? "")
                    .Replace("{Description}", offer?.Description)
                    .Replace("{CreatedAt}", offer?.CreatedAt.ToString())
                    .Replace("{ResponseAt}", DateTime.Now.ToString())
                    .Replace("{ReportLink}", "")
                    .Replace("{EndQuote}", endQuote)
                    .Replace("{projectName}", _configManager.ProjectName);

                _ = Task.Run(async () => await _emailService.SendEmail(new List<string> { influencerUser.Email }, subject, body));
            }
            catch (Exception ex)
            {
                _loggerService.Error("Lỗi khi gửi mail trạng thái thanh toán" + ex);
            }
        }

        public async Task SendMailStatus(Job job, string status, string resultMessage)
        {
            try
            {
                string subject = "Thông báo trạng thái Công Việc";
                var influencerUser = job.Influencer.User;
                var brandUser = job.Campaign.Brand.User;
                var offer = job.Offers.FirstOrDefault(f => f.Status == (int)EOfferStatus.Done);

                var body = _emailTemplate.brandPaymentOffer
                    .Replace("{InfluencerName}", influencerUser.DisplayName)
                    .Replace("{CampaignName}", job.Campaign.Name)
                    .Replace("{JobStatus}", status)
                    .Replace("{JobTitle}", ((EContentType)offer!.ContentType).GetEnumDescription())
                    .Replace("{Description}", _configManager.ProjectName)
                    .Replace("{Price}", offer.Price.ToString("N0"))
                    .Replace("{StartDate}", job.Campaign.StartDate.ToString())
                    .Replace("{EndDate}", job.Campaign.EndDate.ToString())
                    .Replace("{JobDetailsLink}", "")
                    .Replace("{ResultMessage}", resultMessage)
                    .Replace("{projectName}", _configManager.ProjectName);

                _ = Task.Run(async () => await _emailService.SendEmail(new List<string> { influencerUser.Email }, subject, body));
            }
            catch (Exception ex)
            {
                _loggerService.Error("Lỗi khi gửi mail trạng thái Công Việc" + ex);
            }
        }
    }
}
