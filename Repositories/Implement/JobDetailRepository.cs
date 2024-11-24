using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories;

namespace Repositorie
{
    public class JobDetailRepository : IJobDetailRepository
    {
        public async Task Create(JobDetails detail)
        {
            using (var context = new PostgresContext())
            {
                await context.JobDetails.AddAsync(detail);
                await context.SaveChangesAsync();
            }
        }

        public async Task Update(JobDetails detail)
        {
            using (var context = new PostgresContext())
            {
                context.Entry<JobDetails>(detail).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }

        public async Task<JobDetails> GetByDate(DateTime dateTime, Guid jobId)
        {
            using (var context = new PostgresContext())
            {
                var utcDateTime = dateTime.ToUniversalTime();
                var result =  await context.JobDetails
                                    .FirstOrDefaultAsync(j => j.UpdateDate.Date == utcDateTime.Date && j.JobId == jobId);
                return result!;
            }
        }

        public async Task<JobDetails> GetByLinkAndJobId(string link, Guid jobId)
        {
            using (var context = new PostgresContext())
            {
                var result = await context.JobDetails
                                    .FirstOrDefaultAsync(j => j.Link == link && j.JobId == jobId);
                return result!;
            }
        }

        public async Task<IEnumerable<JobDetails>> GetLinkByJobId(Guid jobId)
        {
            using (var context = new PostgresContext())
            {
                var result = await context.JobDetails
                                          .Where(j => j.JobId == jobId)
                                          .ToListAsync();
                return result.DistinctBy(j => j.Link);
            }
        }

    public async Task<(int totalViews, int totalLikes, int totalComments)> GetTotalMetricsByLinkAndJobId(string link, Guid jobId)
        {
            using (var context = new PostgresContext())
            {
                var result = await context.JobDetails
                                        .Where(j => j.Link == link && j.JobId == jobId)
                                        .GroupBy(j => new { j.Link, j.JobId })
                                        .Select(g => new
                                        {
                                            TotalViews = g.Sum(j => j.ViewCount),
                                            TotalLikes = g.Sum(j => j.LikesCount),
                                            TotalComments = g.Sum(j => j.CommentCount)
                                        })
                                        .FirstOrDefaultAsync();

                return result != null
                    ? (result.TotalViews, result.TotalLikes, result.TotalComments)
                    : (0, 0, 0);
            }
        }


    }
}
