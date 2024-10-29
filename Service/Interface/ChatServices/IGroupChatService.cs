

using BusinessObjects.Models;

namespace Service
{
	public interface IGroupChatService
	{
		Task<List<CampaignChat>> GetGroupMessageAsync( Guid roomId);
		Task CreateOrSaveMessageAsync(CampaignChat roomChat);
	}
}
