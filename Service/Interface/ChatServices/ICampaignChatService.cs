

using BusinessObjects;
using BusinessObjects.Models;

namespace Service
{
	public interface ICampaignChatService
	{
        Task<CampaignChatDTO> GetCampaignChatById(Guid campaignChatId);
        Task<CampaignChatDTO> GetCampaignChatByCampaignId(Guid campaignId);
        Task<CampaignChatDTO> CreateCampaignChatRoom(CampaignChatResDTO campaignChat,UserDTO brand);
    }
}
