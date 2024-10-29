

using BusinessObjects.Models;

namespace Repositories
{
	public interface IGroupChatRepository
	{
		Task<List<CampaignChat>> GetGroupMessageAsync( Guid roomId);
		Task CreateOrSaveMessageAsync(CampaignChat roomChat);
	}
}
