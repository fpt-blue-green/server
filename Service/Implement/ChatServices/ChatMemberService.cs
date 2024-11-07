

using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;

namespace Service
{
    public class ChatMemberService : IChatMemberService
    {
        private readonly IChatMemberRepository _chatMemberRepository = new ChatMemberRepository();
        private static readonly IMapper _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        }).CreateMapper();
        public async Task AddNewMember(ChatMemberResDTO chatMemberRes)
        {
            var member = new ChatMember
            {
                CampaignChatId = chatMemberRes.CampaignChatId,
                UserId = chatMemberRes.UserId,
                JoinAt = DateTime.UtcNow
            };
            await _chatMemberRepository.AddNewMember(member);
        }

        public async Task<List<ChatMemberDTO>> GetMembersInCampaign(Guid campaignId)
        {
            var members = await _chatMemberRepository.GetMembersInCampaignChat(campaignId);
            return _mapper.Map<List<ChatMemberDTO>>(members);
        }

        public async Task<bool> CheckExistedMember(Guid userId, Guid campaignChatId)
        {
            var members = await _chatMemberRepository. GetMembersInCampaignChat(campaignChatId);
            var memberIds = members.Select(s => s.UserId).ToList();
            return memberIds.Contains(userId);
        }
    }
}
