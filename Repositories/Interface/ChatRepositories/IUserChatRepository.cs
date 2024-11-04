using BusinessObjects.Models;

namespace Repositories
{
	public interface IUserChatRepository
	{
		Task SaveMessageAsync(UserChat userChat);
		Task<List<UserChat>> GetMessagesAsync(Guid sender, Guid receiver);
	}
}
