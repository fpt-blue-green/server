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
        private static IUserRepository _userRepository = new UserRepository();
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
            var job = await _jobService.GetJobOfferById(jobId);

            var offer = job.Offers.FirstOrDefault(f => f.Status == (int)EOfferStatus.WaitingPayment);
            if (offer == null)
            {
                throw new InvalidOperationException("Không có Offer nào được chấp thuận.");
            }

            var user = await _userRepository.GetUserById(userDto.Id);
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
            }
        }

        public async Task BrandCancleJob(Guid jobId)
        {
            var job = await _jobService.GetJobOfferById(jobId);

            var offer = job.Offers.FirstOrDefault(f => f.Status == (int)EOfferStatus.WaitingPayment);
            if (offer == null)
            {
                throw new InvalidOperationException("Không có Offer nào được chấp thuận.");
            }

            job.Status = (int)EJobStatus.NotCreated;
            job.Offers.FirstOrDefault(f => f.Status == (int)EOfferStatus.WaitingPayment)!.Status = (int)EOfferStatus.Cancelled;
            await _jobService.UpdateJobAndOffer(job);
        }
    }
}
