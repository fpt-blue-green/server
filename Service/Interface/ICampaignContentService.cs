

using BusinessObjects;

namespace Service
{
	public interface ICampaignContentService
	{
		Task CreateCampaignContents(Guid campaignId, List<CampaignContentDto> campaignContents);
		//Task<string> UpdateCampaignContent(Guid campaignId, Guid campaignContentId, CampaignContentResDto campaignContentDto);
		Task<List<CampaignContentDto>> GetCampaignContents(Guid campaignId);
		Task<CampaignContentDto> GetCampaignContent(Guid campaignContentId);

	}
}
