using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Repositories;
using Serilog;
using Service.Helper;
using System.Reflection;
using System.Transactions;
using static BusinessObjects.JobEnumContainer;

namespace Service
{
    public class CampaignService : ICampaignService
    {
        private static readonly IBrandRepository _brandRepository = new BrandRepository();
        private static readonly ICampaignRepository _campaignRepository = new CampaignRepository();
        private static readonly IInfluencerRepository _influencerRepository = new InfluencerRepository();
        private static readonly IEmbeddingRepository _embeddingRepository = new EmbeddingRepository();
        private static readonly ICampaignImageRepository _campaignImagesRepository = new CampaignImageRepository();
        private static readonly ICampaignMeetingRoomService _campaignMeetingRoomService = new CampaignMeetingRoomService();
        private static readonly IJobRepository _jobRepository = new JobRepository();
        private readonly IMapper _mapper;
        private static readonly ILogger _loggerService = new LoggerService().GetDbLogger();
        private static readonly ConfigManager _configManager = new ConfigManager();
        private static readonly EmailTemplate _emailTemplate = new EmailTemplate();
        private static readonly IEmailService _emailService = new EmailService();
        private static readonly EmbeddingUpdater embeddingUpdater = new EmbeddingUpdater();

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
                    if (campaignDto.StartDate == null || campaignDto.EndDate == null)
                    {
                        throw new InvalidOperationException("Chiến dịch phải có đủ ngày bắt đầu và kết thúc.");
                    }

                    var brand = await _brandRepository.GetByUserId(userId);
                    var campaigns = await _campaignRepository.GetByBrandId(brand.Id);

                    if (brand.IsPremium == false && campaigns.Count > 1)
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
            var campaignDuplicateNames = (await _campaignRepository.GetByBrandId(brand.Id)).Where(s => s.Id != campaignId && string.Equals(s.Name, campaignDto.Name, StringComparison.OrdinalIgnoreCase));
            if (campaignDuplicateNames.Any())
            {
                throw new InvalidOperationException("Tên chiến dịch không được trùng lặp.");
            }
            var campaign = await _campaignRepository.GetById(campaignId);

            ValidateBrandAccess(brand.Id, campaign.Brand.Id);
            if (campaign.Status == (int)ECampaignStatus.Draft || campaign.Status == (int)ECampaignStatus.Published)
            {
                _mapper.Map(campaignDto, campaign);
                await _campaignRepository.Update(campaign);
                await embeddingUpdater.UpdateCampaignEmbedding(campaign.Id);
                _loggerService.Information("Cập nhật chiến dịch thành công");
                return campaign.Id;
            }
            throw new InvalidOperationException("Chỉ có thể chỉnh sửa chiến dịch chuẩn bị hoặc đang tuyển thành viên");
        }

        public async Task DeleteCampaign(Guid campaignId, UserDTO userDTO)
        {
            var campaign = await _campaignRepository.GetById(campaignId);
            if (campaign == null)
            {
                throw new KeyNotFoundException("Chiến dịch không tồn tại.");
            }

            var currentBrand = await _brandRepository.GetByUserId(userDTO.Id);
            ValidateBrandAccess(currentBrand.Id, campaign.Brand.Id);

            if (campaign.Status == (int)ECampaignStatus.Active)
            {
                throw new InvalidOperationException("Chiến dịch này đang hoạt động, không thể xoá.");
            }
            if (campaign.Status == (int)ECampaignStatus.Published)
            {
                var jobs = await _jobRepository.GetCampaignJobs(campaign.Id);
                if (jobs.Any(s => s.Status == (int)EJobStatus.InProgress))
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
            var campaignInprogres = campaigns.Where(s => s.Status == (int)ECampaignStatus.Active || s.Status == (int)ECampaignStatus.Published);
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
                    catch { }

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
                    var propertyInfo = typeof(Campaign).GetProperty(filter.SortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo != null)
                    {
                        campaignInprogres = filter.IsAscending.HasValue && filter.IsAscending.Value
                            ? campaignInprogres.OrderBy(i => propertyInfo.GetValue(i, null))
                            : campaignInprogres.OrderByDescending(i => propertyInfo.GetValue(i, null));
                    }
                }
                totalCount = campaignInprogres.Count();
                #region paging
                int pageSize = filter.PageSize;
                campaignInprogres = campaignInprogres
                    .Skip((filter.PageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
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

        public async Task UpdateTagsForCampaign(Guid campaignId, List<Guid> tagIds, UserDTO userDTO)
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
                var currentBrand = await _brandRepository.GetByUserId(userDTO.Id);
                ValidateBrandAccess(currentBrand.Id, campaign.Brand.Id);

                var existingTags = await _campaignRepository.GetTagsOfCampaign(campaignId);
                var deleteTags = existingTags.Where(p => !tagIds.Contains(p.Id)).ToList();
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
                await embeddingUpdater.UpdateCampaignEmbedding(campaignId);
            }
        }

        public async Task<List<string>> UploadCampaignImages(Guid campaignId, List<Guid> imageIds, List<IFormFile> contentFiles, string folder, UserDTO userDTO)
        {
            var contentDownloadUrls = new List<string>();

            var campaign = await _campaignRepository.GetById(campaignId);
            if (campaign == null)
            {
                throw new InvalidOperationException("Chiến dịch không tồn tại");
            }

            var currentBrand = await _brandRepository.GetByUserId(userDTO.Id);

            ValidateBrandAccess(currentBrand.Id, campaign.Brand.Id);

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

        public async Task PublishCampaign(Guid campaignId, UserDTO userDTO)
        {
            var currentBrand = await _brandRepository.GetByUserId(userDTO.Id);
            var campaign = await _campaignRepository.GetById(campaignId);

            ValidateBrandAccess(currentBrand.Id, campaign.Brand.Id);

            if (campaign.Status != (int)ECampaignStatus.Draft)
            {
                throw new InvalidOperationException("Chỉ những chiến dịch đang có trạng thái đang chuẩn bị mới có thể công khai.");
            }
            campaign.Status = (int)ECampaignStatus.Published;
            await _campaignRepository.Update(campaign);
            await embeddingUpdater.UpdateCampaignEmbedding(campaignId);
        }

        public async Task StartCampaign(Guid campaignId, UserDTO userDTO)
        {
            var currentBrand = await _brandRepository.GetByUserId(userDTO.Id);
            var campaign = await _campaignRepository.GetFullDetailCampaignJobById(campaignId) ?? throw new KeyNotFoundException();

            ValidateBrandAccess(currentBrand.Id, campaign.Brand.Id);

            if (campaign.Status != (int)ECampaignStatus.Published)
            {
                throw new InvalidOperationException("Chỉ những chiến dịch đang có trạng thái công khai mới có thể bắt đầu.");
            }

            var jobs = campaign.Jobs.Where(job => job.Status == (int)EJobStatus.Approved).ToList();

            if (jobs?.Any() != true)
            {
                throw new InvalidOperationException("Cần có ít nhất 1 công việc đã được thanh toán để có thể bắt đầu chiến dịch.");
            }

            foreach (var job in jobs)
            {
                job.Status = (int)EJobStatus.InProgress;
            }

            campaign!.Status = (int)ECampaignStatus.Active;
            campaign.StartDate = DateTime.Now;
            await _campaignRepository.Update(campaign);

            // Gửi mail thông báo trong một tác vụ nền
            _ = Task.Run(async () => await SendNotificationStartCampagin(campaign, "Đã Bắt Đầu"));
        }

        public async Task EndCampaign(Guid campaignId, UserDTO userDTO)
        {
            var currentBrand = await _brandRepository.GetByUserId(userDTO.Id);
            var campaign = await _campaignRepository.GetFullDetailCampaignJobById(campaignId) ?? throw new KeyNotFoundException();

            ValidateBrandAccess(currentBrand.Id, campaign.Brand.Id);

            if (campaign.Status != (int)ECampaignStatus.Active)
            {
                throw new InvalidOperationException("Lỗi, chỉ những chiến dịch đang hoạt động mới có thể kết thúc.");
            }

            var runningJob = campaign.Jobs.Where(j => j.Status == (int)EJobStatus.InProgress
            || j.Status == (int)EJobStatus.Pending).ToList();

            if (runningJob.Any())
            {
                throw new InvalidOperationException("Tồn tại các công việc chưa hoàn thành hoặc đang đàm phán. Vui lòng đợi cho các công việc hoàn thành hoặc bạn có thể thay đổi trạng thái của công việc đó.");
            }

            campaign!.Status = (int)ECampaignStatus.Completed;
            campaign.EndDate = DateTime.Now;
            await _campaignRepository.Update(campaign);

            // Gửi mail thông báo trong một tác vụ nền
            _ = Task.Run(async () => await SendNotificationStartCampagin(campaign, "Đã Kết thúc"));
        }

        public async Task SendNotificationStartCampagin(Campaign campaign, string status)
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

                string subject = "Thông Báo Campaign " + status;

                var body = _emailTemplate.campaignStart
                    .Replace("{CampaignName}", campaign?.Name)
                    .Replace("{Title}", campaign?.Title)
                    .Replace("{status}", status)
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

        protected void ValidateBrandAccess(Guid currentBrand, Guid authorBrand)
        {
            if (currentBrand != authorBrand)
            {
                throw new AccessViolationException();
            }
        }

        public async Task<List<UserDTO>> GetCampaignParticipantInfluencer(Guid campaignId)
        {
            var user = await _campaignRepository.GetInfluencerParticipant(campaignId);
            return _mapper.Map<List<UserDTO>>(user);
        }

        public async Task<List<InfluencerDTO>> GetRecommendInfluencers(Guid id)
        {
            var embedding = await _embeddingRepository.GetEmbeddingByCampaignId(id);
            if (embedding == null || embedding.EmbeddingValue == null)
            {
                return new List<InfluencerDTO>();
            }

            var influencers = await _influencerRepository.GetSimilarInfluencers(embedding.EmbeddingValue, 10, 1);
            var influencersDTO = _mapper.Map<List<InfluencerDTO>>(influencers);
            return influencersDTO;
        }

        public async Task<List<CampaignDTO>> GetRecommendCampaigns(Guid userId)
        {
            var influencer = await _influencerRepository.GetByUserId(userId);
            var embedding = await _embeddingRepository.GetEmbeddingByInfluencerId(influencer.Id);
            if (embedding == null || embedding.EmbeddingValue == null)
            {
                return new List<CampaignDTO>();
            }
            var campaigns = await _campaignRepository.GetSimilarCampaigns(embedding.EmbeddingValue);
            var campaignsDTO = _mapper.Map<List<CampaignDTO>>(campaigns);
            return campaignsDTO;
        }
    }
}
