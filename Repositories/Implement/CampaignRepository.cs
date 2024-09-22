using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
	public class CampaignRepository : ICampaignRepository
	{
		public async Task AddTagToCampaign(Guid campaignId, Guid tagId)
		{
			using (var context = new PostgresContext())
			{
				var campaign = await context.Campaigns.Include(i => i.Tags)
											  .FirstOrDefaultAsync(i => i.Id == campaignId);

				var tag = await context.Tags.FirstOrDefaultAsync(t => t.Id == tagId);

				if (campaign != null && tag != null)
				{
					campaign.Tags.Add(tag);
					await context.SaveChangesAsync();
				}
			}
		}

		public async Task Create(Campaign campaign)
		{
			using (var context = new PostgresContext())
			{
				await context.Campaigns.AddAsync(campaign);
				await context.SaveChangesAsync();
			}
		}

		public async Task Delete(Guid id)
		{
			using (var context = new PostgresContext())
			{
				var campaign = await context.Campaigns.FirstOrDefaultAsync(i => i.Id == id);
				if (campaign != null)
				{
					context.Campaigns.Remove(campaign);
					await context.SaveChangesAsync();
				}
			}
		}
		public async Task<IEnumerable<Campaign>> GetAlls()
		{
			using (var context = new PostgresContext())
			{
				var campaigns = await context.Campaigns.ToListAsync();
				return campaigns;
			}
		}

		public async Task<IEnumerable<Campaign>> GetByBrandIdId(Guid id)
		{
			using (var context = new PostgresContext())
			{
				var campaigns = await context.Campaigns.Where(s => s.BrandId == id).ToListAsync();
				return campaigns;
			}
		}

		public async Task<Campaign> GetById(Guid id)
		{
			using (var context = new PostgresContext())
			{
				var campaign = await context.Campaigns.FirstOrDefaultAsync(i => i.Id == id);
				return campaign;
			}
		}

		public async Task<List<Tag>> GetTagsOfCampaign(Guid campaignId)
		{
			using (var context = new PostgresContext())
			{
				var campaign = await context.Campaigns.Include(i => i.Tags)
												.FirstOrDefaultAsync(i => i.Id == campaignId);
				return campaign?.Tags.ToList() ?? new List<Tag>();
			}
		}

		public async Task RemoveTagOfCampaign(Guid campaignId, Guid tagId)
		{
			using (var context = new PostgresContext())
			{
				var campaign = await context.Campaigns.Include(i => i.Tags)
											  .FirstOrDefaultAsync(i => i.Id == campaignId);

				var tag = await context.Tags.FirstOrDefaultAsync(t => t.Id == tagId);

				if (campaign != null && tag != null)
				{
					campaign.Tags.Remove(tag);
					await context.SaveChangesAsync();
				}
			}
		}

		public async Task Update(Campaign campaign)
		{
			using (var context = new PostgresContext())
			{
				context.Entry<Campaign>(campaign).State = EntityState.Modified;
				await context.SaveChangesAsync();
			}
		}
	}
}
