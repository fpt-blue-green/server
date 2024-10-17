using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;
using Serilog;
using static BusinessObjects.JobEnumContainer;

namespace Service
{
    public class JobService : IJobService
    {
        private static IJobRepository _jobService = new JobRepository();
        private static IJobDetailService _jobDetailService = new JobDetailService();
        private static readonly EmailTemplate _emailTemplate = new EmailTemplate();
        private static readonly IEmailService _emailService = new EmailService();
        private static IUserRepository _userRepository = new UserRepository();
        private static IPaymentBookingRepository _paymentBookingRepository = new PaymentBookingRepository();
        private static readonly ConfigManager _configManager = new ConfigManager();
        private static ILogger _loggerService = new LoggerService().GetDbLogger();

        private static readonly IMapper _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        }).CreateMapper();

        public async Task CreateJob(JobDTO job)
        {
            var jobnew = _mapper.Map<Job>(job);
            await _jobService.Create(jobnew);
        }

        public async Task BrandPaymentJob(Guid jobId, UserDTO userDto)
        {
            var job = await _jobService.GetJobFullDetailById(jobId);

            var offer = job.Offers.FirstOrDefault(f => f.Status == (int)EOfferStatus.WaitingPayment);
            if (offer == null)
            {
                throw new InvalidOperationException("Không có đề nghị nào được chấp thuận.");
            }

            var user = job.Campaign.Brand.User;

            if (userDto.Id != user.Id)
            {
                throw new AccessViolationException($"Nhãn hàng với Id {user.Id} đang thanh toán có Id bị bất thường {userDto.Id}");
            }

            if (user.Wallet < offer.Price)
            {
                throw new InvalidOperationException("Không đủ tiền để thanh toán. Vui lòng đến trang nạp tiền để hoàn thành đề nghị.");
            }

            user.Wallet -= offer.Price;
            await _userRepository.UpdateUser(user);
            job.Status = (int)EJobStatus.InProgress;
            job.Offers.FirstOrDefault(f => f.Status == (int)EOfferStatus.WaitingPayment)!.Status = (int)EOfferStatus.Done;
            await _jobService.UpdateJobAndOffer(job);

            //Save Payment data.
            await _paymentBookingRepository.Create(new PaymentBooking
            {
                JobId = jobId,
                Amount = offer.Price,
                Type = (int)EPaymentBookingStatus.BrandPayment,
            });

            var influencerUser = job.Influencer.User;

            //send mail
            await SendMail(job, offer, "được thanh toán", "được thanh toán", "Bạn có thể bắt đầu công việc ngay khi chiến dịch được khởi động.");
        }

        public async Task BrandCancelJob(Guid jobId, UserDTO userDto)
        {
            var job = await _jobService.GetJobFullDetailById(jobId);

            var offer = job.Offers.FirstOrDefault(f => f.Status == (int)EOfferStatus.WaitingPayment);
            if (offer == null)
            {
                throw new InvalidOperationException("Không có đề nghị nào được chấp thuận.");
            }
            var user = job.Campaign.Brand.User;
            if (userDto.Id != user.Id)
            {
                throw new AccessViolationException($"Nhãn hàng với Id {user.Id} đang thanh toán có Id bị bất thường {userDto.Id}");
            }

            job.Status = (int)EJobStatus.NotCreated;
            job.Offers.FirstOrDefault(f => f.Status == (int)EOfferStatus.WaitingPayment)!.Status = (int)EOfferStatus.Cancelled;
            await _jobService.UpdateJobAndOffer(job);

            //send mail
            await SendMail(job, offer, "bị từ chối thanh toán", "bị từ chối", "Rất tiếc, thanh toán cho chiến dịch đã bị hủy, do đó bạn không thể bắt đầu công việc.");
        }

        public async Task AttachPostLink(Guid jobId, UserDTO userDTO, JobLinkDTO linkDTO)
        {
            var job = await _jobService.GetJobInProgress(jobId);
            if (job == null)
            {
                throw new InvalidOperationException("Công việc này chưa bắt đầu hoặc đã kết thúc, không thể thêm liên kết");
            }

            if (job.Influencer.User.Id != userDTO.Id)
            {
                throw new AccessViolationException();
            }

            await _jobDetailService.UpdateJobDetailData(job, linkDTO.Link);
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
    }
}
