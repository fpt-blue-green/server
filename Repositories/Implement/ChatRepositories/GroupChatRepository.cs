

using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
	public class GroupUserChatRepository : IGroupUserChatRepository
	{
		public async Task CreateOrSaveMessageAsync(CampaignChat userChat)
		{
			using (var _context = new PostgresContext())
			{
				await _context.CampaignChats.AddAsync(userChat);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<List<CampaignChat>> GetGroupMessageAsync(string roomName)
		{
			using (var _context = new PostgresContext())
			{
				return await _context.CampaignChats
				.Include(s => s.Sender)
				.Where(c => c.RoomName != null && c.RoomName.ToLower() == roomName.ToLower())
				.OrderBy(c => c.SendTime)
				.ToListAsync();
			}
		}
		public async Task<CampaignChat> GetLastMessage(Guid campaignId, Guid senderId)
		{
			using (var _context = new PostgresContext())
			{
				var lastMessage = await _context.CampaignChats
				.Include(s => s.Sender)
					.Where(c => c.CampaignId == campaignId && c.SenderId == senderId)
					.OrderByDescending(c => c.SendTime)
					.FirstOrDefaultAsync();
				return lastMessage;
			}
		}
	}
}
