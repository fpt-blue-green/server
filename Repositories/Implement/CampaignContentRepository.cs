

using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
	public class CampaignContentRepository : ICampaignContentRepository
	{
		public async Task CreateList(List<CampaignContent> campaignContents)
		{
			using (var context = new PostgresContext())
			{
				await context.CampaignContents.AddRangeAsync(campaignContents);
				await context.SaveChangesAsync();
			}
		}

		public async Task Delete(Guid id)
		{
			using (var context = new PostgresContext())
			{
				var campaignContent = await context.CampaignContents.FirstOrDefaultAsync(i => i.Id == id);
				if (campaignContent != null)
				{
					context.CampaignContents.Remove(campaignContent);
					await context.SaveChangesAsync();
				}
			}
		}

		public async Task<IEnumerable<CampaignContent>> GetAlls()
		{
			using (var context = new PostgresContext())
			{
				var campaignContents = await context.CampaignContents.ToListAsync();
				return campaignContents;
			}
		}

		public async Task<CampaignContent> GetById(Guid id)
		{
			using (var context = new PostgresContext())
			{
				var campaignContent = await context.CampaignContents.FirstOrDefaultAsync(i => i.Id == id);
				return campaignContent!;
			}
		}

		public async Task Update(CampaignContent campaignContent)
		{
			using (var context = new PostgresContext())
			{
				context.Entry(campaignContent).State = EntityState.Modified;
				await context.SaveChangesAsync();
			}
		}
	}
}
