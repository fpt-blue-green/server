

using BusinessObjects.Models;

namespace Repositories
{
	public interface IGroupChatRepository
	{
		Task<List<CampaignChat>> GetGroupMessageAsync(string roomName);
		Task CreateOrSaveMessageAsync(CampaignChat roomChat);
	}
}
