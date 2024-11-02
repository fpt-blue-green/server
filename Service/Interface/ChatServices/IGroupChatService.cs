

using BusinessObjects;
using BusinessObjects.Models;

namespace Service
{
	public interface IGroupChatService
	{
		Task<List<CampaignChatDTO>> GetGroupMessageAsync(string roomName);
		Task CreateOrSaveMessageAsync(CampaignChatDTO userChat);
		Task<CampaignChatDTO> GetLastMessage(Guid campaignId, Guid senderId);
	}
}
