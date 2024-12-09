using AutoMapper;
using BusinessObjects;
using BusinessObjects.DTOs;
using BusinessObjects.Models;
using Repositorie;
using Repositories;
using System.Globalization;
using System.Reflection;
using System.Web;
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
                var links = await _jobRepository.GetLinkJobAppovedInProgress(job.Id);

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

            var oldData = await GetOldJobDetailData(link, job.Id);
            data.ViewCount = Math.Max(0, data.ViewCount - oldData.totalViews);
            data.CommentCount = Math.Max(0, data.CommentCount - oldData.totalComments);
            data.LikesCount = Math.Max(0, data.LikesCount - oldData.totalLikes);

            var jobDetail = _mapper.Map<JobDetails>(data);
            jobDetail.JobId = job.Id;
            jobDetail.Link = link;
            if (isJobUpdate == false)
            {
                jobDetail.IsApprove = false;
            }
            else
            {
                jobDetail.IsApprove = true;
                if (offer.TargetReaction <= data.ViewCount + data.LikesCount + data.CommentCount)
                {
                    job.Status = (int)EJobStatus.Completed;
                    job.CompleteAt = DateTime.Now;
                    await _jobRepository.UpdateJobAndOffer(job);
                }
            }
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

            return new FilterListResponse<CampaignJobDetailDTO>
            {
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
            int targetReaction = data.Jobs.Sum(j => j.Offers.Where(o => o.Status == (int)EOfferStatus.Done).Sum(o => o.TargetReaction)); // Tổng mục tiêu tương tác
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

        public async Task<List<JobPlatFormPieChartDTO>> GetCampaignJobDetailPlatForm(Guid campaignId)
        {
            // Lấy dữ liệu từ repository
            var data = await _campaignRepository.GetCampaignJobDetails(campaignId);

            if (data == null || data.Jobs == null)
            {
                // Nếu không có dữ liệu, trả về danh sách rỗng
                return new List<JobPlatFormPieChartDTO>();
            }

            // Khởi tạo dictionary với tất cả các nền tảng có giá trị mặc định là 0
            var platformStats = Enum.GetValues(typeof(EPlatform))
                .Cast<EPlatform>()
                .ToDictionary(platform => platform, platform => 0L); // Giá trị mặc định là 0L (long)

            // Lấy danh sách tất cả các Offers có liên kết đến Job
            var offers = data.Jobs
                .Where(job => job.Offers != null) // Loại bỏ các Job không có Offers
                .SelectMany(job => job.Offers)
                .Where(offer => offer.Job?.JobDetails != null && offer.Status == (int)EOfferStatus.Done); // Loại bỏ các Offer không có JobDetails

            // Nhóm dữ liệu theo Platform và tính tổng ViewCount, LikesCount và CommentCount
            var calculatedStats = offers
                .GroupBy(o => o.Platform)
                .ToDictionary(
                    g => g.Key, // Platform
                    g => g.Sum(o => o.Job.JobDetails.Sum(jd => jd.ViewCount + jd.LikesCount + jd.CommentCount)) // Tổng tương tác
                );

            // Gộp dữ liệu từ `calculatedStats` vào `platformStats`
            foreach (var stat in calculatedStats)
            {
                if (platformStats.ContainsKey((EPlatform)stat.Key))
                {
                    platformStats[(EPlatform)stat.Key] = stat.Value;
                }
            }

            // Chuyển đổi dictionary thành danh sách JobPlatFormPieChartDTO
            var result = platformStats
                .Select(ps => new JobPlatFormPieChartDTO
                {
                    Platform = ps.Key,
                    Value = ps.Value
                })
                .OrderBy(dto => dto.Platform) // Sắp xếp theo Platform (tùy ý)
                .ToList();

            return result;
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

        public async Task<Dictionary<int, int>> GetCampaignJobStatus(Guid campaignId)
        {
            // Lấy dữ liệu job chi tiết cho campaign từ repository
            var data = await _campaignRepository.GetCampaignAllJobDetails(campaignId);

            // Khởi tạo Dictionary với các giá trị số tương ứng với các trạng thái
            var jobStatusCounts = new Dictionary<int, int>
            {
                { 0, 0 },  // Peding
                { 1, 0 },  // Budgeting
                { 2, 0 },  // Joined
                { 3, 0 },  // Fulfillment
                { 4, 0 }   // Archived
            };

            if (data == null)
            {
                return jobStatusCounts;
            }

            // Cập nhật số lượng job thực tế cho các trạng thái có trong dữ liệu
            foreach (var job in data.Jobs)
            {
                // Lấy offer cuối cùng của job
                var lastOffer = job.Offers.OrderByDescending(o => o.CreatedAt).FirstOrDefault();

                if (lastOffer != null)
                {
                    var status = (EJobStatus)job.Status;
                    var offerStatus = (EOfferStatus)lastOffer.Status;

                    // Kiểm tra trạng thái và ánh xạ sang các giá trị số tương ứng
                    if (status == EJobStatus.Pending && offerStatus == EOfferStatus.Offering)
                    {
                        jobStatusCounts[0]++;  // Peding
                    }
                    else if (status == EJobStatus.Pending && offerStatus == EOfferStatus.WaitingPayment)
                    {
                        jobStatusCounts[1]++;  // Budgeting
                    }
                    else if ((status == EJobStatus.Approved || status == EJobStatus.InProgress) && offerStatus == EOfferStatus.Done)
                    {
                        jobStatusCounts[2]++;  // Joined
                    }
                    else if (status == EJobStatus.Completed)
                    {
                        jobStatusCounts[3]++;  // Fulfillment
                    }
                    else if (status == EJobStatus.Failed)
                    {
                        jobStatusCounts[3]++;  // Fulfillment
                    }
                    else if (status == EJobStatus.NotCreated)
                    {
                        jobStatusCounts[4]++;  // Archived
                    }
                    else
                    {
                        jobStatusCounts[4]++;  // Archived
                    }
                }
            }

            return jobStatusCounts;
        }

        #endregion

        #region Job Statistic
        public async Task<List<CampaignDailyStatsDTO>> GetJobDailyStats(Guid jobId, string? link)
        {
            var allJobDetails = new List<JobDetails>();

            if (link != null)
            {
                link = HttpUtility.UrlDecode(link);
                allJobDetails = await _campaignRepository.GetDailyJobStatus(jobId, link);
            }
            else
            {
                allJobDetails = await _campaignRepository.GetAllDailyJobStatus(jobId);
            }


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

        public async Task<CampaignJobDetailBaseDTO> GetJobDetailBaseData(Guid jobId, string? link)
        {
            var allJobDetails = new List<JobDetails>();

            if (link != null)
            {
                link = HttpUtility.UrlDecode(link);
                allJobDetails = await _campaignRepository.GetDailyJobStatus(jobId, link);
            }
            else
            {
                allJobDetails = await _campaignRepository.GetAllDailyJobStatus(jobId);
            }

            int totalView = allJobDetails.Sum(jd => jd.ViewCount);
            int totalLike = allJobDetails.Sum(jd => jd.LikesCount);
            int totalComment = allJobDetails.Sum(jd => jd.CommentCount);
            int totalReaction = totalView + totalLike + totalComment;

            // Lấy danh sách công việc
            var jobs = allJobDetails.Select(jd => jd.Job).Distinct().ToList();

            int targetReaction = jobs.Sum(j => j.Offers.Where(o => o.Status == (int)EOfferStatus.Done).Sum(o => o.TargetReaction));
            decimal totalFee = jobs.Sum(j => j.Offers.Where(o => o.Status == (int)EOfferStatus.Done).Sum(o => o.Price));

            return new CampaignJobDetailBaseDTO
            {
                TotalView = totalView,
                TotalLike = totalLike,
                TotalComment = totalComment,
                TotalReaction = totalReaction,
                TargetReaction = targetReaction,
                TotalFee = totalFee
            };
        }
        #endregion
    }
}
