

using BusinessObjects;
using BusinessObjects.Models;

namespace Service
{
	public interface ICampaignChatService
	{
        Task<CampaignChatDTO> GetCampaignChatById(Guid campaignChatId);
        Task CreateCampaignChatRoom(CampaignChatResDTO campaignChat);
    }
}
