
using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;
using Repositories.Implement;
using Repositories.Interface;
using Serilog;
using Supabase.Gotrue;

namespace Service
{
	public class CampaignService : ICampaignService
	{
		private static readonly IBrandRepository _brandRepository = new BrandRepository();
		private static readonly ICampaignRepository _campaignRepository = new CampaignRepository();
		private readonly IMapper _mapper;
		private static ILogger _loggerService = new LoggerService().GetDbLogger();
		public CampaignService(IMapper mapper)
		{
			_mapper = mapper;
		}
		public async Task<string> CreateCampaign(Guid userId, CampaignDTO campaignDto)
		{
			var brand = await _brandRepository.GetByUserId(userId);
			var campaign = new Campaign();
			if (campaignDto.Id == null)
			{
				//create
				campaign = _mapper.Map<Campaign>(campaignDto);
				campaign.BrandId = brand.Id;
				await _campaignRepository.Create(campaign);
			}
			else
			{
				//update
				campaign = _mapper.Map<Campaign>(campaignDto);
				campaign.BrandId = brand.Id;
				await _campaignRepository.Update(campaign);
			}
			_loggerService.Information("Tạo campaign thành công");
			return "Tạo thành công";
		}

		public async Task<CampaignDTO> GetBrandCampaign(Guid userId, Guid campaignId)
		{
			var result = new CampaignDTO();
			var brand = await _brandRepository.GetByUserId(userId);
			if (brand != null)
			{
				var campaigns = await _campaignRepository.GetByBrandIdId(brand.Id);
				if (campaigns.Count() > 0)
				{
					var campaign = campaigns.FirstOrDefault(s => s.Id == campaignId);
					if (campaign == null)
					{
						throw new KeyNotFoundException("Campaign không tồn tại.");
					}
					result = _mapper.Map<CampaignDTO>(campaign);
				}
			}
			return result;
		}

		public async Task<List<CampaignDTO>> GetBrandCampaigns(Guid userId)
		{
			var result = new List<CampaignDTO>();
			var brand = await _brandRepository.GetByUserId(userId);
			if (brand != null)
			{
				var campaigns = await _campaignRepository.GetByBrandIdId(brand.Id);
				if (campaigns.Count() > 0)
				{
					result = _mapper.Map<List<CampaignDTO>>(campaigns);
				}
			}
			return result;
		}

		public async Task<string> UpdateCampaign(Guid userId, CampaignDTO campaignDto)
		{
			var brand = await _brandRepository.GetByUserId(userId);
			var campaign = await _campaignRepository.GetById((Guid)campaignDto.Id);

			_mapper.Map(campaignDto, campaign);
			await _campaignRepository.Update(campaign);
			_loggerService.Information("Cập nhật campaign thành công");
			return "Cập nhật campaign thành công";
		}

		public async Task<List<CampaignBrandDto>> GetAllCampaigns()
		{
			var result = new List<CampaignBrandDto>();
			var campaigns = await _campaignRepository.GetAlls();
			if (campaigns.Count() > 0)
			{
				result = _mapper.Map<List<CampaignBrandDto>>(campaigns);
			}
			return result;
		}
		public async Task<List<TagDTO>> GetTagsOfCampaign(Guid campaignId)
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
		}
		public async Task<string> UpdateTagsForCampaign(Guid campaignId, List<Guid> tagIds)
		{
			var duplicateTagIds = tagIds.GroupBy(t => t)
								.Where(g => g.Count() > 1)
								.Select(g => g.Key)
								.ToList();
			if (duplicateTagIds.Any())
			{
				throw new InvalidOperationException("Tag không được trùng lặp.");
			}
			var campaign = await _campaignRepository.GetById(campaignId);

			if (campaign == null)
			{
				throw new InvalidOperationException("Không tìm thấy campaign.");
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
			return "Cập nhật tag thành công.";
		}
	}
}
