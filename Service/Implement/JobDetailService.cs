using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositorie;
using Repositories;
using static BusinessObjects.JobEnumContainer;

namespace Service
{
    public class JobDetailService : IJobDetailService
    {
        private static IUtilityService _utilityService = new UtilityService();
        private static IJobDetailRepository _jobDetailRepository = new JobDetailRepository();
        private static IJobRepository _jobRepository = new JobRepository();
        private static readonly IMapper _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        }).CreateMapper();


        public async Task UpdateJobDetailData(Job job, string? link)
        {
            // Nếu link không null, tạo 1 JobDetail mới
            if (link != null)
            {
                await CreateSingleJobDetails(job, link);
            }
            else
            {
                var links = await _jobRepository.GetLinkJobInProgress(job.Id);

                foreach(var item in links)
                {
                    await CreateSingleJobDetails(job, item);
                }
            }
        }

        public static async Task CreateSingleJobDetails(Job job, string link)
        {
            var offer = job.Offers.FirstOrDefault(f => f.Status == (int)EOfferStatus.Done);
            if (offer == null)
            {
                throw new InvalidOperationException("Không có Offer nào trong Job.");
            }

            var data = await _utilityService.GetVideoInformation(offer!.Platform!, link);
            var oldData = await GetJobDetailByDate(DateTime.Now.AddDays(-1), job.Id);
            if(oldData != null)
            {
                data.ViewCount -= oldData.ViewCount;
                data.CommentCount -= oldData.CommentCount;
                data.LikesCount -= oldData.LikesCount;
            }

            var jobDetail = _mapper.Map<JobDetail>(data);
            jobDetail.JobId = job.Id;
            jobDetail.Link = link;
            await _jobDetailRepository.Create(jobDetail);
        }

        public static async Task<JobDetail> GetJobDetailByDate(DateTime dateTime, Guid jobId)
        {
            return await _jobDetailRepository.GetByDate(dateTime, jobId);
        }
    }
}
