
using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;
using Repositories.Implement;
using Repositories.Interface;
using Serilog;

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
	}
}
