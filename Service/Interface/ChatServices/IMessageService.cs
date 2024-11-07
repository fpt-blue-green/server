
using BusinessObjects;
using BusinessObjects.Models;

namespace Service
{
    public interface IMessageService
    {
        Task SaveMessageAsync(MessageResDTO messageRes);
        Task<List<MessageDTO>> GetMessagesAsync(Guid senderId, Guid receiverId);
        Task<List<MessageDTO>> GetCampaignMessagesAsync(Guid senderId, Guid campaignChatId);
        Task<MessageDTO> GetLastMessage(Guid senderId, Guid campaignChatId);
        
    }
}
