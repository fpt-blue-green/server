

using BusinessObjects;

namespace Service
{
    public interface IChatContactService
    {
        Task<List<ChatContactDTO>> GetChatContactsAsync(Guid userId);
    }
}
