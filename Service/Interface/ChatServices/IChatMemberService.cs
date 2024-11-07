

using BusinessObjects;
using BusinessObjects.Models;

namespace Service
{
    public interface IChatMemberService
    {
        Task<List<ChatMemberDTO>> GetMembersInCampaign(Guid campaignId);
        Task AddNewMember(ChatMemberResDTO chatMemberRes);
        Task<bool> CheckExistedMember(Guid userId,Guid campaignChatId);

    }
}
