

using BusinessObjects.Models;

namespace Repositories
{
	public interface IGroupUserChatRepository
	{
		Task<List<CampaignChat>> GetGroupMessageAsync(string roomName);
		Task CreateOrSaveMessageAsync(CampaignChat userChat);
		Task<CampaignChat> GetLastMessage(Guid campaignId, Guid senderId);
	}
}
