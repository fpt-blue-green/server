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

    }
}
