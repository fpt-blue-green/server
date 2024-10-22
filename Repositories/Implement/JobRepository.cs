using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using static BusinessObjects.JobEnumContainer;

namespace Repositories
{
    public class JobRepository : IJobRepository
    {
        public async Task<IEnumerable<Job>> GetAllPedingJob()
        {
            using (var context = new PostgresContext())
            {
                var jobs = await context.Jobs
                    .Include(i => i.Offers)
                    .Where(j => j.Status == (int)EJobStatus.Pending)
                    .ToListAsync();
                return jobs;
            }
        }

        public async Task Create(Job job)
        {
            using (var context = new PostgresContext())
            {
                await context.Jobs.AddAsync(job);
                await context.SaveChangesAsync();
            }
        }

        public async Task Update(Job job)
        {
            using (var context = new PostgresContext())
            {
                context.Entry(job).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateJobAndOffer(Job job)
        {
            using (var context = new PostgresContext())
            {
                context.Entry(job).State = EntityState.Modified;

                // Cập nhật trạng thái của từng Offer con trong Job
                foreach (var offer in job.Offers)
                {
                    context.Entry(offer).State = EntityState.Modified;
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task<Job> GetJobFullDetailById(Guid id)
        {
            using (var context = new PostgresContext())
            {
                var job = await context.Jobs.Where(j => j.Id == id)
                                            .Include(j => j.Campaign).ThenInclude( c => c.Brand).ThenInclude(b => b.User)
                                            .Include(j => j.Influencer).ThenInclude(b => b.User)
                                            .Include(j => j.Offers)
                                            .FirstOrDefaultAsync();
                return job!;
            }
        }

        public async Task<Job> GetJobInProgress(Guid id)
        {
            using (var context = new PostgresContext())
            {
                var job = await context.Jobs.Where(j => j.Id == id && j.Status == (int)EJobStatus.InProgress)
                                            .Include(j => j.Offers)
                                            .Include(j => j.Influencer).ThenInclude(i => i.User)
                                            .FirstOrDefaultAsync();
                return job!;
            }
        }

        public async Task<IEnumerable<string>> GetLinkJobInProgress(Guid id)
        {
            using (var context = new PostgresContext())
            {
                var links = await context.Jobs
                                        .Where(j => j.Id == id && j.Status == (int)EJobStatus.InProgress)
                                        .Include(j => j.JobDetails) 
                                        .SelectMany(j => j.JobDetails.Select(jd => jd.Link))
                                        .Distinct()
                                        .ToListAsync();

                return links!;
            }
        }

        public async Task<IEnumerable<Job>> GetAllJobInProgress()
        {
            using (var context = new PostgresContext())
            {
                var jobs = await context.Jobs.Where(j => j.Status == (int)EJobStatus.InProgress && j.Campaign.Status != (int)ECampaignStatus.Completed)
                                            .Include(j => j.Offers)
                                            .Include(j => j.Influencer).ThenInclude(i => i.User)
                                            .ToListAsync();
                return jobs!;
            }
        }

        public async Task<IEnumerable<Job>> FilterJobInfluencerByCampaignStatus(Guid userId, int eCampaignStatus)
        {
            using (var context = new PostgresContext())
            {
                var jobs = await context.Jobs.Where(j => j.Campaign.Status == eCampaignStatus && j.Influencer.UserId == userId)
                                            .Include(j => j.Offers)
                                            .Include(j => j.Influencer).ThenInclude(i => i.User)
                                            .ToListAsync();
                return jobs!;
            }
        }

        public async Task<IEnumerable<Job>> FilterJobBrandByCampaignStatus(Guid userId, int eCampaignStatus)
        {
            using (var context = new PostgresContext())
            {
                var jobs = await context.Jobs.Where(j => j.Campaign.Status == eCampaignStatus && j.Campaign.Brand.UserId == userId)
                                            .Include(j => j.Offers)
                                            .Include(j => j.Influencer).ThenInclude(i => i.User)
                                            .ToListAsync();
                return jobs!;
            }
        }

        public async Task<IEnumerable<Job>> FilterJobInfluencerByJobStatus(Guid userId, int eJobStatus)
        {
            using (var context = new PostgresContext())
            {
                var jobs = await context.Jobs.Where(j => j.Status == eJobStatus && j.Influencer.UserId == userId)
                                            .Include(j => j.Offers)
                                            .Include(j => j.Influencer).ThenInclude(i => i.User)
                                            .ToListAsync();
                return jobs!;
            }
        }

        public async Task<IEnumerable<Job>> FilterJobBrandByJobStatus(Guid userId, int eJobStatus)
        {
            using (var context = new PostgresContext())
            {
                var jobs = await context.Jobs.Where(j => j.Status == eJobStatus && j.Campaign.Brand.UserId == userId)
                                            .Include(j => j.Offers)
                                            .Include(j => j.Influencer).ThenInclude(i => i.User)
                                            .ToListAsync();
                return jobs!;
            }
        }

        public async Task<IEnumerable<Job>> GetCampaignJobs(Guid campaginId)
        {
            using (var context = new PostgresContext())
            {
                var jobs = await context.Jobs.Where(j => j.CampaignId == campaginId)
                                            .Include(j => j.Offers)
                                            .Include(j => j.Influencer).ThenInclude(i => i.User)
                                            .ToListAsync();
                return jobs!;
            }
        }

        public async Task<IEnumerable<Job>> GetJobInfluencerByUserId(Guid userId)
        {
            using (var context = new PostgresContext())
            {
                var jobs = await context.Jobs.Where(j => j.Influencer.UserId == userId)
                                            .Include(j => j.Influencer)
                                            .Include(j => j.Campaign)
                                            .ToListAsync();
                return jobs!;
            }
        }

        public async Task<IEnumerable<Job>> GetJobBrandByUserId(Guid userId)
        {
            using (var context = new PostgresContext())
            {
                var jobs = await context.Jobs.Where(j => j.Campaign.Brand.UserId == userId)
                                            .Include(j => j.Influencer)
                                            .Include(j => j.Campaign)
                                            .ToListAsync();
                return jobs!;
            }
        }
    }
}
