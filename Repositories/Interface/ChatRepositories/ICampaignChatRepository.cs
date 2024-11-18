

using BusinessObjects.Models;

namespace Repositories
{
	public interface ICampaignChatRepository
	{
		Task<CampaignChat> GetCampaignChatById(Guid campaignChatId);
		Task<Guid> CreateCampaignChatRoom(CampaignChat campaignChat);
	}
}
