
using BusinessObjects;

namespace Service
{
	public interface ICampaignService
	{
		Task<string> CreateCampaign(Guid userId,CampaignDTO campaign);
		Task<string> UpdateCampaign(Guid userId, CampaignDTO campaign);
		Task<List<CampaignDTO>> GetBrandCampaigns(Guid userId);
		Task<CampaignDTO> GetBrandCampaign(Guid userId,Guid campaignId);
	}
}
