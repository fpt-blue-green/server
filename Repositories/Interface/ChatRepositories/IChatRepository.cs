using BusinessObjects.Models;

namespace Repositories
{
	public interface IChatRepository
	{
		Task SaveMessageAsync(ChatRoom roomChat);
		Task<List<ChatRoom>> GetMessagesAsync(Guid sender, Guid receiver);
	}
}
