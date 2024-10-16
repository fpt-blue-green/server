using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using Serilog;

namespace Service
{
    public class CampaignContentService : ICampaignContentService
    {
        private static readonly ICampaignContentRepository _campaignContentRepository = new CampaignContentRepository();
        private static readonly ICampaignRepository _campaignRepository = new CampaignRepository();
        private static ILogger _loggerService = new LoggerService().GetDbLogger();
        public async Task CreateCampaignContents(Guid campaginId, List<CampaignContentDto> campaignContents)
        {
            var createContents = new List<CampaignContentDto>();
            var updateContents = new List<CampaignContentDto>();
            var deleteContents = new List<CampaignContentDto>();
            //get all content of campaign
            var allContents = await GetCampaignContents(campaginId);
            var allContentsIds = allContents.Select(p => p.Id).ToList();

            foreach (var content in campaignContents)
            {
                if (content.Id.HasValue)
                {
                    //nếu thằng mô có id thì allway update
                    updateContents.Add(content);
                }
                else
                {
                    //thằng mô ko có id thì create
                    createContents.Add(content);
                }
            }
            var campaign = await _campaignRepository.GetById(campaginId);
            if (campaign == null)
            {
                throw new InvalidOperationException("campaign không tồn tại.");
            }
            if (updateContents.Any())
            {
                foreach (var content in updateContents)
                {
                    var existingContent = await _campaignContentRepository.GetById(content.Id!.Value);
                    if (existingContent != null)
                    {
                        existingContent.Description = content.Description;
                        existingContent.Platform = (int)content.Platform;
                        existingContent.ContentType = (int)content.ContentType;
                        existingContent.Quantity = content.Quantity;
                        existingContent.Price = content.Price;
                        await _campaignContentRepository.Update(existingContent);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Content với ID {content.Id.Value} không tồn tại.");
                    }
                }
            }
            if (createContents.Any())
            {

                var contentNeedCreate = new List<CampaignContent>();
                foreach (var content in createContents)
                {
                    var c = new CampaignContent
                    {
                        CampaignId = campaign.Id,
                        Description = content.Description,
                        ContentType = (int)content.ContentType,
                        Platform = (int)content.Platform,
                        Quantity = content.Quantity,
                        Price = content.Price,
                    };
                    contentNeedCreate.Add(c);
                }
                if (contentNeedCreate == null || contentNeedCreate.Count == 0)
                {
                    throw new InvalidOperationException("Vui lòng tạo ít nhất 1 content.");
                }
                contentNeedCreate.ForEach(content => content.CampaignId = campaign.Id);

                await _campaignContentRepository.CreateList(contentNeedCreate);
                _loggerService.Information("Tạo content thành công");
            }
            // Tìm các content có trong all contents nhưng không có trong list contents
            var listDtoContentIds = campaignContents
                .Where(p => p.Id.HasValue)
                .Select(p => p.Id!.Value)
                .ToList();

            deleteContents = allContents.Where(p => !listDtoContentIds.Contains((Guid)p.Id!)).ToList();
            var deleteIds = deleteContents.Select(p => p.Id).ToList();
            if (deleteIds.Any())
            {
                var contentIdNeedDeletes = new List<Guid>();
                foreach (var id in deleteIds)
                {
                    contentIdNeedDeletes.Add((Guid)id!);
                }
                foreach (var contentId in contentIdNeedDeletes)
                {
                    await _campaignContentRepository.Delete(contentId);
                }
                _loggerService.Information("Xoá content không còn tồn tại thành công");
            }
        }

        public async Task<CampaignContentDto> GetCampaignContent(Guid campaignContentId)
        {

            var camapaignContent = await _campaignContentRepository.GetById(campaignContentId);
            if (camapaignContent == null)
            {
                _loggerService.Information($"get campaign content {campaignContentId} thất bại");
                throw new InvalidOperationException("Campaign content không tồn tại.");

            }
            _loggerService.Information($"get campaign content {campaignContentId} thành công");
            var result = new CampaignContentDto
            {
                Id = campaignContentId,
                Description = camapaignContent.Description,
                Quantity = camapaignContent.Quantity,
                Platform = (EPlatform)camapaignContent.Platform,
                ContentType = (EContentType)camapaignContent.ContentType,
                Price = camapaignContent.Price,
            };
            return result;
        }

        public async Task<List<CampaignContentDto>> GetCampaignContents(Guid campaignId)
        {
            var result = new List<CampaignContentDto>();
            var campaign = (await _campaignRepository.GetById(campaignId));
            if (campaign == null)
            {
                _loggerService.Information("Campaign không tồn tại.");
                throw new InvalidOperationException("Campaign không tồn tại.");
            }
            var campaignContents = (await _campaignContentRepository.GetAlls()).Where(s => s.CampaignId == campaign.Id);
            if (campaignContents.IsNullOrEmpty())
            {
                _loggerService.Information($"get campaign content : {campaignId} không có tồn tại : count = 0");
                return new List<CampaignContentDto>();
            }
            _loggerService.Information($"get campaign content : {campaignId} thành công");
            foreach (var item in campaignContents)
            {
                result.Add(new CampaignContentDto
                {
                    Id = item.Id,
                    Description = item.Description,
                    ContentType = (EContentType)item.ContentType,
                    Platform = (EPlatform)item.Platform,
                    Quantity = item.Quantity,
                    Price = item.Price,
                });
            }
            return result;
        }

        /*public async Task<string> UpdateCampaignContent(Guid campaignId, Guid campaignContentId, CampaignContentResDto campaignContentDto)
		{
			var content = await _campaignContentRepository.GetById(campaignContentId);
			var campaign = await _campaignRepository.GetById(campaignId);
			if (campaign == null)
			{
				_loggerService.Information("campaign không tồn tại.");
				throw new InvalidOperationException("campaign không tồn tại.");
			}
			if (content == null)
			{
				throw new InvalidOperationException("Không tìm thấy content.");
			}
			if (content.CampaignId == campaign.Id)
			{
				content.Platform = (int)campaignContentDto.Platform;
				content.Quantity = campaignContentDto.Quantity;
				content.Content = campaignContentDto.Content;
				content.ContentType = (int)campaignContentDto.ContentType;
				await _campaignContentRepository.Update(content);
				_loggerService.Information($"cập nhật campaign content: {campaignId} thành công");
			}
			else
			{
				_loggerService.Information($"cập nhật campaign content : {campaignId} thất bại.");
				throw new InvalidOperationException("Không thể cập nhật.");
			}
			return "Cập nhật thành công";
		}*/
    }
}
