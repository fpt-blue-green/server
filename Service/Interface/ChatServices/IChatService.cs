
using BusinessObjects.Models;

namespace Service
{
    public interface IChatService
    {
        Task SaveMessageAsync(UserChat userChat);
        Task<List<UserChat>> GetMessagesAsync(Guid sender, Guid receiver);
    }
}
