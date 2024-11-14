

using BusinessObjects;

namespace Service
{
    public interface IChatContactService
    {
        Task<List<ChatPartnerDTO>> GetChatContactsAsync(Guid userId,string? searchValue );
        Task<ChatPartnerDTO> GetChatContactByIdAsync(Guid chatId);
    }
}
