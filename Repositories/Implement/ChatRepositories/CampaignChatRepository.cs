﻿

using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
	public class CampaignChatRepository : ICampaignChatRepository
	{
		public async Task<CampaignChat> CreateCampaignChatRoom(CampaignChat campaignChat)
		{
			using (var _context = new PostgresContext())
			{
				await _context.CampaignChats.AddAsync(campaignChat);
				await _context.SaveChangesAsync();
				return campaignChat;
			}
		}

		public async Task<CampaignChat> GetCampaignChatById(Guid campaignChat)
		{
			using (var _context = new PostgresContext())
			{
				var chat =  await _context.CampaignChats
                .Include(s => s.ChatMembers).FirstOrDefaultAsync(c => c.Id == campaignChat);
				return chat!;
			}
		}

		public async Task<CampaignChat> GetCampaignChatByCampaignId(Guid campaignId)
		{
			using (var _context = new PostgresContext())
			{
				var chat =  await _context.CampaignChats
                .Include(s => s.ChatMembers).FirstOrDefaultAsync(c => c.CampaignId == campaignId);
				return chat!;
			}
		}
	}
}
