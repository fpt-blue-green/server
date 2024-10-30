

using BusinessObjects.Models;

namespace Service
{
	public interface IGroupChatService
	{
		Task<List<CampaignChat>> GetGroupMessageAsync(string roomName);
		Task CreateOrSaveMessageAsync(CampaignChat roomChat);
	}
}
