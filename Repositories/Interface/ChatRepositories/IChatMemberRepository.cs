using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IChatMemberRepository
    {
        Task<List<ChatMember>> GetMembersInCampaignChat(Guid campaignId);
        Task AddNewMember(ChatMember chatMember);
    }
}
