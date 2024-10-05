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

        public async Task<Job> GetJobOfferById(Guid id)
        {
            using (var context = new PostgresContext())
            {
                var job = await context.Jobs.Where(j => j.Id == id)
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
                var jobs = await context.Jobs.Where(j => j.Status == (int)EJobStatus.InProgress)
                                            .Include(j => j.Offers)
                                            .Include(j => j.Influencer).ThenInclude(i => i.User)
                                            .ToListAsync();
                return jobs!;
            }
        }
    }
}
