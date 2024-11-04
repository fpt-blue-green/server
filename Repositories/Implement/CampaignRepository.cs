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

		public async Task Delete(Campaign campaign)
		{
			using (var context = new PostgresContext())
			{
				if (campaign != null)
				{
					campaign.IsDeleted = true;
                    context.Entry<Campaign>(campaign).State = EntityState.Modified;
                    await context.SaveChangesAsync();
				}
			}
		}
		public async Task<IEnumerable<Campaign>> GetAlls()
		{
			using (var context = new PostgresContext())
			{
				var campaigns = await context.Campaigns
					.Include(s => s.Brand).ThenInclude(s => s.User)
                    .Include(s => s.Tags)
					.Include(s => s.CampaignImages)
					.Include(s => s.CampaignMeetingRooms)
					.Include(s => s.CampaignContents).ToListAsync();
				return campaigns!;
			}
		}

		public async Task<List<Campaign>> GetByBrandId(Guid id)
		{
			using (var context = new PostgresContext())
			{
				var campaigns = await context.Campaigns
					.Include(s => s.Brand).ThenInclude(s => s.User)
					.Include(s => s.Tags)
					.Include(s => s.CampaignImages)
					.Include(s => s.CampaignMeetingRooms)
					.Include(s => s.CampaignContents)
                    .Where(s => s.BrandId == id).ToListAsync();
				return campaigns;
			}
		}

		public async Task<Campaign> GetById(Guid id)
		{
			using (var context = new PostgresContext())
			{
				var campaign = await context.Campaigns
					.Include(s => s.Brand).ThenInclude(s => s.User)
                    .Include(s => s.Tags)
					.Include(s => s.CampaignImages)
					.Include(s => s.CampaignMeetingRooms)
					.Include(s => s.CampaignContents)
                    .FirstOrDefaultAsync(i => i.Id == id);
				return campaign;
			}
		}

		public async Task<Campaign> GetFullDetailCampaignJobById(Guid id)
		{
            using (var context = new PostgresContext())
            {
                var campaign = await context.Campaigns
                    .Include(s => s.Brand).ThenInclude(s => s.User)
                    .Include(s => s.Jobs).ThenInclude(s => s.Influencer).ThenInclude(s => s.User)
                    .FirstOrDefaultAsync(i => i.Id == id);
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
                foreach (var job in campaign.Jobs)
                {
                    context.Entry(job).State = EntityState.Modified;
                }
                await context.SaveChangesAsync();
			}
		}

        public async Task<Campaign> GetCampaignJobDetails(Guid campaignId)
        {
            using (var context = new PostgresContext())
            {
                // Lấy campaign theo campaignId
                var campaign = await context.Campaigns
                                            .Include(c => c.Jobs)
                                                .ThenInclude(j => j.Offers)
                                            .Include(c => c.Jobs)
                                                .ThenInclude(j => j.JobDetails)
											.Include(c => c.Jobs)
												.ThenInclude(j => j.Influencer)
													.ThenInclude(i => i.User)
                                            .Where(c => c.Id == campaignId).FirstOrDefaultAsync();
                return campaign;

            }

        }
    }
}
