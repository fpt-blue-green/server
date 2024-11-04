using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositorie;
using Repositories;
using System.Globalization;
using System.Reflection;
using static BusinessObjects.JobEnumContainer;

namespace Service
{
    public class JobDetailService : IJobDetailService
    {
        private static IUtilityService _utilityService = new UtilityService();
        private static IJobDetailRepository _jobDetailRepository = new JobDetailRepository();
        private static IJobRepository _jobRepository = new JobRepository();
        private static ICampaignRepository _campaignRepository = new CampaignRepository();
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

                foreach (var item in links)
                {
                    await CreateSingleJobDetails(job, item, true);
                }
            }
        }

        public static async Task CreateSingleJobDetails(Job job, string link, bool isJobUpdate = false)
        {
            var offer = job.Offers.FirstOrDefault(f => f.Status == (int)EOfferStatus.Done);
            if (offer == null)
            {
                if (isJobUpdate)
                {
                    return;
                }
                throw new InvalidOperationException("Không có đề nghị nào đã thanh toán trong công việc.");
            }

            var data = await _utilityService.GetVideoInformation(offer!.Platform!, link);

            if(offer.TargetReaction > 0 && (data.LikesCount + data.ViewCount + data.CommentCount) >= offer.TargetReaction)
            {
                if(isJobUpdate)
                {
                    job.Status = (int)EJobStatus.Completed;
                    job.CompleteAt = DateTime.Now;
                    await _jobRepository.Update(job);
                    return;
                }
            }
            else if (offer.TargetReaction > 0 && (data.LikesCount + data.ViewCount + data.CommentCount) < offer.TargetReaction)
            {
                if(job.Campaign.EndDate.ToUniversalTime() <= DateTime.Now.ToUniversalTime())
                {
                    job.Status = (int)EJobStatus.Failed;
                    job.CompleteAt = DateTime.Now;
                    await _jobRepository.Update(job);
                    return;
                }
            }

            var oldData = await GetOldJobDetailData(link, job.Id);
            data.ViewCount -= oldData.totalViews;
            data.CommentCount -= oldData.totalComments;
            data.LikesCount -= oldData.totalLikes;

            var jobDetail = _mapper.Map<JobDetails>(data);
            jobDetail.JobId = job.Id;
            jobDetail.Link = link;
            await _jobDetailRepository.Create(jobDetail);
        }

        public static async Task<(int totalViews, int totalLikes, int totalComments)> GetOldJobDetailData(string link, Guid jobId)
        {
            return await _jobDetailRepository.GetTotalMetricsByLinkAndJobId(link, jobId);
        }

        #region Campagin Statistic
        public async Task<FilterListResponse<CampaignJobDetailDTO>> GetCampaignJobDetail(Guid campaignId, FilterDTO filter)
        {
            var data = await _campaignRepository.GetCampaignJobDetails(campaignId);

            var campaignJobDetails = new List<CampaignJobDetailDTO>();

            // Nhóm jobs theo influencer
            var influencerGroups = data.Jobs.GroupBy(j => j.Influencer.Id);

            foreach (var group in influencerGroups)
            {
                var influencer = group.First().Influencer; // Lấy influencer từ nhóm
                var totalJobCount = group.Count(); // Tổng số job của influencer
                var totalView = group.Sum(j => j.JobDetails.Sum(jd => jd.ViewCount)); // Tổng lượt xem
                var totalLike = group.Sum(j => j.JobDetails.Sum(jd => jd.LikesCount)); // Tổng lượt thích
                var totalComment = group.Sum(j => j.JobDetails.Sum(jd => jd.CommentCount)); // Tổng bình luận
                var totalReaction = totalComment + totalView + totalLike;
                var targetReaction = group.Sum(j => j.Offers.Sum(jd => jd.TargetReaction)); // Tổng mục tiêu tương tác
                var totalFee = group.Sum(j => j.Offers.Sum(jd => jd.Price)); // Tổng mục tiêu tương tác

                // Thêm thông tin vào danh sách CampaignJobDetailDTO
                campaignJobDetails.Add(new CampaignJobDetailDTO
                {
                    Name = influencer.FullName + $"({influencer.Slug})"!,
                    Avatar = influencer.User.Avatar!,
                    TotalJob = totalJobCount,
                    TotalView = totalView,
                    TotalLike = totalLike,
                    TotalComment = totalComment,
                    TotalReaction = totalReaction,
                    TargetReaction = targetReaction,
                    TotalFee = totalFee,
                });
            }


            #region Sort
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                var propertyInfo = typeof(CampaignJobDetailDTO).GetProperty(filter.SortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    campaignJobDetails = filter.IsAscending.HasValue && filter.IsAscending.Value
                        ? campaignJobDetails.OrderBy(i => propertyInfo.GetValue(i, null)).ToList()
                        : campaignJobDetails.OrderByDescending(i => propertyInfo.GetValue(i, null)).ToList();
                }
            }
            #endregion

            int totalCount = campaignJobDetails.Count();
            #region paging
            int pageSize = filter.PageSize;
            campaignJobDetails = campaignJobDetails
                .Skip((filter.PageIndex - 1) * pageSize)
                .Take(pageSize).ToList();
            #endregion

            return new FilterListResponse<CampaignJobDetailDTO>{
                Items = campaignJobDetails,
                TotalCount = totalCount,
            }; 
        }

        public async Task<CampaignJobDetailBaseDTO> GetCampaignJobDetailBaseData(Guid campaignId)
        {
            var data = await _campaignRepository.GetCampaignJobDetails(campaignId);
            int totalJobCount = data.Jobs.Count;
            int totalView = data.Jobs.Sum(j => j.JobDetails.Sum(jd => jd.ViewCount));
            int totalLike = data.Jobs.Sum(j => j.JobDetails.Sum(jd => jd.LikesCount));
            int totalComment = data.Jobs.Sum(j => j.JobDetails.Sum(jd => jd.CommentCount));
            int totalReaction = totalView + totalLike + totalComment; // Tổng tương tác
            int targetReaction = data.Jobs.Sum(j => j.Offers.Sum(o => o.TargetReaction)); // Tổng mục tiêu tương tác
            decimal totalFee = data.Jobs.Sum(j => j.Offers.Sum(o => o.Price)); // Tổng chi phí

            return new CampaignJobDetailBaseDTO
            {
                TotalJob = totalJobCount,
                TotalView = totalView,
                TotalLike = totalLike,
                TotalComment = totalComment,
                TotalReaction = totalReaction,
                TargetReaction = targetReaction,
                TotalFee = totalFee
            };
        }

        public async Task<List<CampaignDailyStatsDTO>> GetCampaignDailyStats(Guid campaignId)
        {
            // Lấy dữ liệu chiến dịch bao gồm tất cả các công việc và chi tiết công việc
            var campaignData = await _campaignRepository.GetCampaignJobDetails(campaignId);

            // Lấy tất cả các JobDetail từ tất cả các Jobs trong chiến dịch
            var allJobDetails = campaignData.Jobs.SelectMany(job => job.JobDetails);

            // Kiểm tra nếu không có JobDetails, trả về danh sách rỗng
            if (!allJobDetails.Any())
            {
                return new List<CampaignDailyStatsDTO>();
            }

            // Lấy ngày nhỏ nhất và ngày lớn nhất từ các JobDetails
            var startDate = allJobDetails.Min(jd => jd.UpdateDate.Date);
            var endDate = allJobDetails.Max(jd => jd.UpdateDate.Date);

            // Tạo danh sách các ngày từ ngày nhỏ nhất đến ngày lớn nhất
            var allDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                                     .Select(offset => startDate.AddDays(offset))
                                     .Select(date => date.ToString("dd/MM/yyyy")) // Định dạng ngày
                                     .ToList();

            // Thống kê số liệu cho mỗi ngày
            var dailyStats = allDates.Select(dateStr => new CampaignDailyStatsDTO
            {
                Date = dateStr,
                TotalReaction = allJobDetails
                                .Where(jd => jd.UpdateDate.ToString("dd/MM/yyyy") == dateStr) // So sánh ngày
                                .Sum(jd => jd.ViewCount + jd.LikesCount + jd.CommentCount) // Tính tổng phản hồi
            })
            .OrderBy(dailyStat => DateTime.ParseExact(dailyStat.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture)) // Sắp xếp theo ngày
            .ToList();

            return dailyStats!;
        }

        #endregion

    }
}
