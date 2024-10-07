using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;
using static BusinessObjects.JobEnumContainer;

namespace Service
{
    public class JobService : IJobService
    {
        private static IJobRepository _jobService = new JobRepository();
        private static IJobDetailService _jobDetailService = new JobDetailService();
        private static readonly EmailTemplate _emailTempalte = new EmailTemplate();
        private static readonly IEmailService _emailService = new EmailService();
        private static IUserRepository _userRepository = new UserRepository();
        private static IInfluencerRepository _influencerService = new InfluencerRepository();
        private static IPaymentBookingRepository _paymentBookingRepository = new PaymentBookingRepository();

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
                throw new InvalidOperationException("Không có Offer nào được chấp thuận.");
            }
            var user = job.Campaign.Brand.User;

            if (userDto.Id != user.Id)
            {
                throw new AccessViolationException($"Brand với Id {user.Id} đang thanh toán có Id bị bất thường {userDto.Id}");
            }

            if (user.Wallet < offer.Price)
            {
                throw new InvalidOperationException("Không đủ tiền để thanh toán. Vui lòng đến trang nạp tiền để hoàn thành Offer.");
            }
            else
            {
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

            }
        }

        public async Task BrandCancleJob(Guid jobId, UserDTO userDto)
        {
            var job = await _jobService.GetJobFullDetailById(jobId);

            var offer = job.Offers.FirstOrDefault(f => f.Status == (int)EOfferStatus.WaitingPayment);
            if (offer == null)
            {
                throw new InvalidOperationException("Không có Offer nào được chấp thuận.");
            }
            var user = job.Campaign.Brand.User;
            if (userDto.Id != user.Id)
            {
                throw new AccessViolationException($"Brand với Id {user.Id} đang thanh toán có Id bị bất thường {userDto.Id}");
            }

            job.Status = (int)EJobStatus.NotCreated;
            job.Offers.FirstOrDefault(f => f.Status == (int)EOfferStatus.WaitingPayment)!.Status = (int)EOfferStatus.Cancelled;
            await _jobService.UpdateJobAndOffer(job);
        }

        public async Task AttachPostLink(Guid jobId, UserDTO userDTO, JobLinkDTO linkDTO)
        {
            var job = await _jobService.GetJobInProgress(jobId);
            if (job == null)
            {
                throw new InvalidOperationException("Job này chưa bắt đầu hoặc đã kết thúc, không thể thêm link");
            }

            if (job.Influencer.User.Id != userDTO.Id)
            {
                throw new AccessViolationException();
            }

            await _jobDetailService.UpdateJobDetailData(job, linkDTO.Link);
        }

        public async Task SendMail(Job job)
        {
            string subject = string.Empty;
            string body = string.Empty;
            await _emailService.SendEmail(new List<string> { "" }, subject, body);
        }
    }
}
