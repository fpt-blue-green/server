

using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
	public class GroupChatRepository : IGroupChatRepository
	{
		public async Task CreateOrSaveMessageAsync(CampaignChat roomChat)
		{
			using(var _context = new PostgresContext())
			{
				await _context.CampaignChats.AddAsync(roomChat);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<List<CampaignChat>> GetGroupMessageAsync( string roomName)
		{
			using (var _context = new PostgresContext())
			{
				return await _context.CampaignChats
				.Where(c =>c.RoomName!= null  && c.RoomName.ToLower() == roomName.ToLower())
				.OrderBy(c => c.SendTime)
				.ToListAsync();
			}
		}
	}
}
