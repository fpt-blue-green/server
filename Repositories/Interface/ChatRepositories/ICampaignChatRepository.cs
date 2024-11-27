

using BusinessObjects.Models;

namespace Repositories
{
	public interface ICampaignChatRepository
	{
		Task<CampaignChat> GetCampaignChatById(Guid campaignChatId);
		Task<CampaignChat> GetCampaignChatByCampaignId(Guid campaignId);
		Task<CampaignChat> CreateCampaignChatRoom(CampaignChat campaignChat);
	}
}
