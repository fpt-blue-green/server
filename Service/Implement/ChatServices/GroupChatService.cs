

using BusinessObjects.Models;
using Repositories;

namespace Service
{
	public class GroupChatService : IGroupChatService
	{
		private readonly IGroupChatRepository _groupChatRepository = new GroupChatRepository();
		public async Task CreateOrSaveMessageAsync(CampaignChat roomChat)
		{
			await _groupChatRepository.CreateOrSaveMessageAsync(roomChat);
		}

		public async Task<List<CampaignChat>> GetGroupMessageAsync( Guid roomId)
		{
			return await _groupChatRepository.GetGroupMessageAsync( roomId);
		}
	}
}
