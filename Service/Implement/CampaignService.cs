using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Repositories;
using Serilog;
using Service.Helper;
using Supabase.Gotrue;
using System.Linq;
using System.Reflection;
using System.Transactions;
using static BusinessObjects.JobEnumContainer;
using static Quartz.Logging.OperationName;

namespace Service
{
    public class CampaignService : ICampaignService
    {
        private static readonly IBrandRepository _brandRepository = new BrandRepository();
        private static readonly ICampaignRepository _campaignRepository = new CampaignRepository();
        private static readonly ICampaignImageRepository _campaignImagesRepository = new CampaignImageRepository();
        private static readonly ICampaignMeetingRoomService _campaignMeetingRoomService = new CampaignMeetingRoomService();
        private static readonly IJobRepository _jobRepository = new JobRepository();
        private readonly IMapper _mapper;
        private static readonly ILogger _loggerService = new LoggerService().GetDbLogger();
        private static readonly ConfigManager _configManager = new ConfigManager();
        private static readonly EmailTemplate _emailTemplate = new EmailTemplate();
        private static readonly IEmailService _emailService = new EmailService();

        public CampaignService(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task<Guid> CreateCampaign(Guid userId, CampaignResDto campaignDto)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if(campaignDto.StartDate == null || campaignDto.EndDate == null)
                    {
                        throw new InvalidOperationException("Chiến dịch phải có đủ ngày bắt đầu và kết thúc.");
                    }

                    var brand = await _brandRepository.GetByUserId(userId);
                    var campaigns = (await _campaignRepository.GetByBrandId(brand.Id));

                    if (brand.IsPremium == false && campaigns.Count > 2)
                    {
                        throw new InvalidOperationException("Tài khoản hiện tại chỉ có thể tạo 2 chiến dịch. Vui lòng nâng cấp để tiếp tục sử dụng.");
                    }

                    if (campaigns.Where(s => string.Equals(s.Name, campaignDto.Name, StringComparison.OrdinalIgnoreCase)).Any())
                    {
                        throw new InvalidOperationException("Tên chiến dịch không được trùng lặp.");
                    }
                    var campaign = new Campaign();
                    /*if (campaignDto.Id == null)
                    {*/
                    //create
                    campaign = _mapper.Map<Campaign>(campaignDto);
                    campaign.BrandId = brand.Id;
                    campaign.Status = (int)ECampaignStatus.Draft;
                    await _campaignRepository.Create(campaign);
                    /*}
                    else
                    {
                        //update
                        campaign = _mapper.Map<Campaign>(campaignDto);
                        campaign.BrandId = brand.Id;
                        await _campaignRepository.Update(campaign);
                    }*/

                    await _campaignMeetingRoomService.CreateFirstTimeRoom(campaign.Id);
                    _loggerService.Information("Tạo campaign thành công");

                    scope.Complete();

                    return campaign.Id;
                }
                catch
                {
                    throw;
                }
            }
        }

        public async Task<CampaignDTO> GetCampaign(Guid campaignId)
        {
            var result = new CampaignDTO();
            var campaign = await _campaignRepository.GetById(campaignId);
            if (campaign == null)
            {
                throw new KeyNotFoundException("Campaign không tồn tại.");
            }

            result = _mapper.Map<CampaignDTO>(campaign);

            return result;
        }

        public async Task<FilterListResponse<CampaignDTO>> GetBrandCampaignsByUserId(Guid userId, BrandCampaignFilterDTO filter)
        {
            var result = new List<CampaignDTO>();
            var totalCount = 0;
            var brand = await _brandRepository.GetByUserId(userId);
            if (brand != null)
            {
                var campaigns = await _campaignRepository.GetByBrandId(brand.Id);
                if (campaigns == null)
                {
                    throw new KeyNotFoundException("Campaign không tồn tại.");
                }

                #region filter
                if (filter.CampaignStatus != null && filter.CampaignStatus.Any())
                {
                    campaigns = campaigns.Where(i => filter.CampaignStatus.Contains((ECampaignStatus)i.Status!)).ToList();
                }
                #endregion

                totalCount = campaigns.Count();
                #region Paging
                int pageSize = filter.PageSize;
                campaigns = campaigns
                    .Skip((filter.PageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                #endregion

                result = _mapper.Map<List<CampaignDTO>>(campaigns);
            }

            return new FilterListResponse<CampaignDTO>
            {
                TotalCount = totalCount,
                Items = result
            };
        }

        public async Task<List<CampaignDTO>> GetAvailableBrandCampaigns(Guid brandId)
        {
            var result = new List<CampaignDTO>();
            var campaign = await _campaignRepository.GetByBrandId(brandId);
            var activeCampaigns = campaign.Where(c => c.Status == (int)ECampaignStatus.Published || c.Status == (int)ECampaignStatus.Active).ToList();
            result = _mapper.Map<List<CampaignDTO>>(activeCampaigns);
            return result;
        }

        public async Task<Guid> UpdateCampaign(Guid userId, Guid campaignId, CampaignResDto campaignDto)
        {
            var brand = await _brandRepository.GetByUserId(userId);
            var campaignDuplicateNames = (await _campaignRepository.GetByBrandId(brand.Id)).Where(s => string.Equals(s.Name, campaignDto.Name, StringComparison.OrdinalIgnoreCase));
            if (campaignDuplicateNames.Any())
            {
                throw new InvalidOperationException("Tên chiến dịch không được trùng lặp.");
            }
            var campaign = await _campaignRepository.GetById(campaignId);

            _mapper.Map(campaignDto, campaign);
            await _campaignRepository.Update(campaign);
            _loggerService.Information("Cập nhật chiến dịch thành công");
            return campaign.Id;
        }

        public async Task DeleteCampaign(Guid campaignId)
        {
            var campaign = await _campaignRepository.GetById(campaignId);
            if (campaign == null)
            {
                throw new KeyNotFoundException("Chiến dịch không tồn tại.");
            }
            if (campaign.Status == (int)ECampaignStatus.Active)
            {
                throw new InvalidOperationException("Chiến dịch này đang hoạt động, không thể xoá.");
            }
            if (campaign.Status == (int)ECampaignStatus.Published)
            {
                var jobs = await _jobRepository.GetCampaignJobs(campaign.Id);
                if (jobs.Any(s => s.Status == (int)JobEnumContainer.EJobStatus.InProgress))
                {
                    throw new InvalidOperationException("Chiến dịch này có công việc đã thanh toán, không thể xóa.");
                }
            }
            else
            {
                await _campaignRepository.Delete(campaign);
            }
        }

        public async Task<FilterListResponse<CampaignDTO>> GetCampaignsInProgress(CampaignFilterDTO filter)
        {
            var result = new List<CampaignDTO>();
            var campaigns = await _campaignRepository.GetAlls();
            var campaignInprogres = campaigns.Where(s => /*(s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now) &&*/ (s.Status == (int)ECampaignStatus.Active || s.Status == (int)ECampaignStatus.Published));
            var totalCount = 0;
            if (campaignInprogres.Any())
            {
                if (filter.TagIds != null && filter.TagIds.Any())
                {
                    campaignInprogres = campaignInprogres.Where(i =>
                        i.Tags.Any(it => filter.TagIds.Contains(it.Id))
                    );
                }
                if (filter.PriceFrom.HasValue && filter.PriceTo.HasValue)
                {
                    try
                    {
                        campaignInprogres = campaignInprogres.Where(i =>
                                                (!filter.PriceFrom.HasValue || i.Budget >= filter.PriceFrom) &&
                                                (!filter.PriceTo.HasValue || i.Budget <= filter.PriceTo));
                    }
                    catch (Exception e) { }

                }
                if (!string.IsNullOrEmpty(filter.Search))
                {
                    campaignInprogres = campaignInprogres.Where(i =>
                        i.Name.Contains(filter.Search, StringComparison.OrdinalIgnoreCase) ||
                        i.Title.Contains(filter.Search, StringComparison.OrdinalIgnoreCase)
                    );
                }
                if (!string.IsNullOrEmpty(filter.SortBy))
                {
                    var propertyInfo = typeof(Influencer).GetProperty(filter.SortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo != null)
                    {
                        campaignInprogres = filter.IsAscending.HasValue && filter.IsAscending.Value
                            ? (List<Campaign>)campaignInprogres.OrderBy(i => propertyInfo.GetValue(i, null))
                            : (List<Campaign>)campaignInprogres.OrderByDescending(i => propertyInfo.GetValue(i, null));
                    }
                }
                totalCount = campaignInprogres.Count();
                #region paging
                int pageSize = filter.PageSize;
                campaignInprogres = campaignInprogres
                    .Skip((filter.PageIndex - 1) * pageSize)
                    .Take(pageSize);
                #endregion
                result = _mapper.Map<List<CampaignDTO>>(campaignInprogres);
            }
            return new FilterListResponse<CampaignDTO>
            {
                TotalCount = totalCount,
                Items = result
            };
        }

        /*public async Task<List<TagDTO>> GetTagsOfCampaign(Guid campaignId)
		{
			var listTagsRes = new List<TagDTO>();
			var campaign = await _campaignRepository.GetById(campaignId);
			if (campaign != null)
			{
				var listTags = await _campaignRepository.GetTagsOfCampaign(campaignId);
				if (listTags != null && listTags.Any())
				{
					listTagsRes = _mapper.Map<List<TagDTO>>(listTags);
					return listTagsRes;
				}
			}
			return listTagsRes;
		}*/

        public async Task UpdateTagsForCampaign(Guid campaignId, List<Guid> tagIds)
        {
            var duplicateTagIds = tagIds.GroupBy(t => t)
                                .Where(g => g.Count() > 1)
                                .Select(g => g.Key)
                                .ToList();
            if (duplicateTagIds.Any())
            {
                throw new InvalidOperationException("Thẻ không được trùng lặp.");
            }
            var campaign = await _campaignRepository.GetById(campaignId);

            if (campaign == null)
            {
                throw new InvalidOperationException("Không tìm thấy chiến dịch.");
            }
            else
            {
                var existingTags = await _campaignRepository.GetTagsOfCampaign(campaignId);
                var deleteTags = existingTags.Where(p => !tagIds.Contains((Guid)p.Id)).ToList();
                var newTags = tagIds.Except(existingTags.Select(t => t.Id)).ToList();
                //create new tag
                if (newTags.Count > 0)
                {
                    foreach (var tagId in newTags)
                    {
                        await _campaignRepository.AddTagToCampaign(campaignId, tagId);
                    }
                }
                //delete tag
                if (deleteTags.Any())
                {
                    foreach (var tag in deleteTags)
                    {
                        await _campaignRepository.RemoveTagOfCampaign(campaignId, tag.Id);
                    }
                }

            }
        }

        public async Task<List<string>> UploadCampaignImages(Guid campaignId, List<Guid> imageIds, List<IFormFile> contentFiles, string folder)
        {
            var contentDownloadUrls = new List<string>();


            var campaign = await _campaignRepository.GetById(campaignId);
            if (campaign == null)
            {
                throw new InvalidOperationException("Chiến dịch không tồn tại");
            }

            // Lấy danh sách các ảnh hiện có của influencer từ DB
            var existingImages = await _campaignImagesRepository.GetByCampaignId(campaign.Id);

            // Tìm các ảnh trùng trong danh sách mới và danh sách hiện có
            var matchingImages = existingImages.Where(image => imageIds.Contains(image.Id)).ToList();

            // Tìm các ảnh không trùng trong danh sách mới và danh sách hiện có
            var unMatchingImages = existingImages.Where(image => !imageIds.Contains(image.Id)).ToList();

            // Kiểm tra nếu số ảnh không trùng và số ảnh mới nhỏ hơn 1
            if (matchingImages.Count + contentFiles.Count < 1)
            {
                throw new InvalidOperationException("Chiến dịch phải có ít nhất 1 ảnh.");
            }

            // Kiểm tra tổng số ảnh sau khi thêm mới không được vượt quá 10
            if (matchingImages.Count + contentFiles.Count > 10)
            {
                throw new InvalidOperationException("Chiến dịch chỉ được có tối đa 10 ảnh.");
            }

            // Nếu điều kiện hợp lệ, xóa các ảnh cũ không nằm trong danh sách mới
            foreach (var unMatchingImage in unMatchingImages)
            {
                var imagePath = CloudinaryHelper.GetValueAfterLastSlash(unMatchingImage.Url);
                var link = $"CampaignImages/{imagePath}";

                // Xóa ảnh trên cloudinary
                var deletionParams = new DeletionParams(link);
                await CloudinaryHelper.RemoveImageAsync(deletionParams);

                // Xóa ảnh từ DB
                await _campaignImagesRepository.Delete(unMatchingImage.Id);

            }

            // Thêm các ảnh mới từ contentFiles vào Cloudinary và DB
            _loggerService.Information("Start to upload content images: ");
            foreach (var file in contentFiles)
            {
                try
                {
                    var contentDownloadUrl = await CloudinaryHelper.UploadImageAsync(file, folder, null);
                    contentDownloadUrls.Add(contentDownloadUrl.ToString());

                    var campaignImage = new CampaignImage
                    {
                        Id = Guid.NewGuid(),
                        CampaignId = campaign.Id,
                        Url = contentDownloadUrl.ToString(),
                    };

                    await _campaignImagesRepository.Create(campaignImage);

                }
                catch (Exception ex)
                {
                    _loggerService.Error(ex, $"Error uploading content image: {file.FileName}");
                    contentDownloadUrls.Add($"Error uploading content {file.FileName}: {ex.Message}");
                }
            }

            _loggerService.Information("End to upload content images: ");
            return contentDownloadUrls;
        }

        public async Task PublishCampaign(Guid campaignId)
        {
            var campaign = await _campaignRepository.GetById(campaignId);
            if (campaign.Status != (int)ECampaignStatus.Published)
            {
                throw new InvalidOperationException("Chỉ những chiến dịch đang có trạng thái đang chuẩn bị mới có thể công khai.");
            }
            campaign.Status = (int)ECampaignStatus.Published;
            await _campaignRepository.Update(campaign);
        }

        public async Task StartCampaign(Guid campaignId)
        {
            var campaign = await _campaignRepository.GetFullDetailCampaignJobById(campaignId) ?? throw new KeyNotFoundException();

            if (campaign.Status != (int)ECampaignStatus.Published)
            {
                throw new InvalidOperationException("Chỉ những chiến dịch đang có trạng thái công khai mới có thể bắt đầu.");
            }

            if (campaign?.Jobs?.Any(s => s.Status == (int)EJobStatus.InProgress) != true)
            {
                throw new InvalidOperationException("Cần có ít nhất 1 công việc đã được thanh toán để có thể bắt đầu chiến dịch.");
            }
            var jobs = campaign.Jobs;
            foreach (var job in jobs)
            {
                job.Status = (int)EJobStatus.InProgress;
            }

            campaign!.Status = (int)ECampaignStatus.Active;
            campaign.StartDate = DateTime.Now;
            await _campaignRepository.Update(campaign);

            // Gửi mail thông báo trong một tác vụ nền
            _ = Task.Run(async () => await SendNotificationStartCampagin(campaign));
        }

        public async Task SendNotificationStartCampagin(Campaign campaign)
        {
            try
            {
                var emails = campaign?.Jobs?
                 .Where(j => j.Influencer != null && j.Influencer.User != null && !string.IsNullOrEmpty(j.Influencer.User.Email))
                 .Select(j => j.Influencer.User.Email)
                 .Distinct()
                 .ToList();

                if (emails == null)
                {
                    return;
                }

                string subject = "Thông Báo Campaign Đã Bắt Đầu";

                var body = _emailTemplate.campaignStart
                    .Replace("{CampaignName}", campaign?.Name)
                    .Replace("{Title}", campaign?.Title)
                    .Replace("{BrandName}", campaign?.Brand?.User?.DisplayName)
                    .Replace("{StartDate}", DateTime.Now.ToString())
                    .Replace("{EndDate}", campaign?.EndDate.ToString())
                    .Replace("{CampaignLink}", "")
                    .Replace("{projectName}", _configManager.ProjectName);
                await _emailService.SendEmail(emails, subject, body);
            }
            catch (Exception ex)
            {
                _loggerService.Error("Lỗi khi gửi mail Start Campaign" + ex);
            }
        }
    }
}
