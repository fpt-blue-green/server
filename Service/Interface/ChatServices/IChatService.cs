
using BusinessObjects.Models;

namespace Service
{
    public interface IChatService
    {
        Task SaveMessageAsync(ChatRoom roomChat);
        Task<List<ChatRoom>> GetMessagesAsync(Guid sender, Guid receiver);
    }
}
