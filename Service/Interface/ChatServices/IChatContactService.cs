

using BusinessObjects;

namespace Service
{
    public interface IChatContactService
    {
        Task<List<ChatPartnerDTO>> GetChatContactsAsync(Guid userId);
        Task<ChatPartnerDTO> GetChatContactByIdAsync(Guid chatId);
    }
}
