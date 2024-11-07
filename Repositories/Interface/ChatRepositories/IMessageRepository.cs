using BusinessObjects.Models;

namespace Repositories
{
	public interface IMessageRepository
	{
		Task SaveMessageAsync(Message message);
		Task<List<Message>> GetMessagesAsync(Guid senderId, Guid receiverId);
        Task<List<Message>> GetCampaignMessagesAsync(Guid senderId, Guid campaignChatId);
		Task<Message> GetLastMessage(Guid senderId, Guid receiverId);
		Task<List<Message>> GetMessagesByUserIdAsync(Guid userId);
    }
}
