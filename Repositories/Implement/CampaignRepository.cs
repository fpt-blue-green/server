using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using static BusinessObjects.JobEnumContainer;

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

        public async Task<IEnumerable<Campaign>> GetAllsIgnoreFilter()
        {
            using (var context = new PostgresContext())
            {
                var campaigns = await context.Campaigns
                    .Include(s => s.Brand).ThenInclude(s => s.User)
                    .Include(s => s.Tags)
                    .Include(s => s.CampaignImages)
                    .Include(s => s.CampaignMeetingRooms)
                    .Include(s => s.CampaignContents).IgnoreQueryFilters().ToListAsync();
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
                    .Where(s => s.BrandId == id && (s.Status == (int) ECampaignStatus.Active || s.Status == (int)ECampaignStatus.Published)).ToListAsync();
				return campaigns;
			}
		}

        public async Task<List<User>> GetInfluencerParticipant(Guid campaignId)
        {
            using (var context = new PostgresContext())
            {
                var participants = await context.Jobs
                    .Where(job => job.CampaignId == campaignId && 
										job.Status != (int)EJobStatus.Pending &&
										job.Status != (int)EJobStatus.NotCreated)
                    .Select(job => job.Influencer.User)
                    .Distinct()
                    .ToListAsync();

                return participants;
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
                // Lấy campaign và lọc thủ công JobDetails sau khi truy vấn
                var campaign = await context.Campaigns
                    .Include(c => c.Jobs.Where(j => j.Status != (int)EJobStatus.NotCreated && j.Status != (int)EJobStatus.Pending))
                        .ThenInclude(j => j.Offers)
                    .Include(c => c.Jobs.Where(j => j.Status != (int)EJobStatus.NotCreated && j.Status != (int)EJobStatus.Pending))
                        .ThenInclude(j => j.JobDetails)
                    .Include(c => c.Jobs.Where(j => j.Status != (int)EJobStatus.NotCreated && j.Status != (int)EJobStatus.Pending))
                        .ThenInclude(j => j.Influencer)
                            .ThenInclude(i => i.User)
                    .Where(c => c.Id == campaignId)
                    .FirstOrDefaultAsync();

                // Lọc JobDetails có IsApprove = true
                foreach (var job in campaign?.Jobs ?? Enumerable.Empty<Job>())
                {
                    job.JobDetails = job.JobDetails.Where(jd => jd.IsApprove).ToList();
                }

                return campaign;
            }
        }


        public async Task<List<JobDetails>>GetDailyJobStatus(Guid jobId, string link)
		{
            using (var context = new PostgresContext())
            {
                var result = await context.JobDetails
										.Where(j => j.JobId == jobId && j.Link == link && j.IsApprove == true)
										.Include(j => j.Job)
										.ToListAsync();
				return result;
            }
        }

        public async Task<List<JobDetails>> GetAllDailyJobStatus(Guid jobId)
        {
            using (var context = new PostgresContext())
            {
                var result = await context.JobDetails
                                        .Where(j => j.JobId == jobId && j.IsApprove == true)
                                        .Include(j => j.Job)
                                        .ToListAsync();
                return result;
            }
        }

        public async Task<Campaign> GetCampaignAllJobDetails(Guid campaignId)
        {
            using (var context = new PostgresContext())
            {
                // Lấy campaign theo campaignId với điều kiện job.Status khác NotCreated
                var campaign = await context.Campaigns
                    .Include(c => c.Jobs)
                        .ThenInclude(j => j.Offers)
                    .Include(c => c.Jobs)
                        .ThenInclude(j => j.JobDetails)
                    .Include(c => c.Jobs)
                        .ThenInclude(j => j.Influencer)
                            .ThenInclude(i => i.User)
                    .Where(c => c.Id == campaignId)
                    .FirstOrDefaultAsync();

                return campaign;
            }
        }

        public async Task<Campaign> GetByCampaignChatId(Guid campaignChatId)
		{
			using (var _context = new PostgresContext())
			{
				var campaignChat = await _context.CampaignChats
					.Include(c => c.Campaign) // Bao gồm Campaign liên quan
					.FirstOrDefaultAsync(c => c.Id == campaignChatId); // Lọc theo campaignChatId

				return campaignChat?.Campaign; // Trả về Campaign từ CampaignChat, hoặc null nếu không tìm thấy
			}
		}
	}
}
