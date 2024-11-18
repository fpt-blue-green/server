

using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class ChatMemberRepository : IChatMemberRepository
    {
        public async Task AddNewMember(ChatMember chatMember)
        {
           using(var _context = new PostgresContext())
            {
                await _context.ChatMembers.AddAsync(chatMember);
                await _context.SaveChangesAsync();
            }
        }

		public async Task DeleteMember(Guid memberId, Guid campaignChatId)
		{
			using (var _context = new PostgresContext())
			{
                var member = await _context.ChatMembers.FirstOrDefaultAsync(s => s.UserId == memberId && s.CampaignChatId == campaignChatId);
				_context.ChatMembers.Remove(member);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<List<ChatMember>> GetMembersInCampaignChat(Guid campaignChatId)
        {
            using (var _context = new PostgresContext())
            {
                var result = (await _context.CampaignChats.Include(c => c.ChatMembers).ThenInclude(u => u.User).FirstOrDefaultAsync(s => s.Id == campaignChatId))!.ChatMembers;
                
                return (List<ChatMember>)result;
            }
        }

		public async Task<List<Guid>> GetMyGroupChat(Guid userId)
		{
			using(var context = new PostgresContext())
            {
                var groups = context.ChatMembers.Where(s => s.UserId == userId);
                return groups.Select(s => s.CampaignChatId).ToList();
            }
		}
	}
}
