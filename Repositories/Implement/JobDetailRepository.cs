using BusinessObjects.Models;
using Repositories;

namespace Repositorie
{
    public class JobDetailRepository : IJobDetailRepository
    {
        public async Task Create(JobDetail detail)
        {
            using (var context = new PostgresContext())
            {
                await context.JobDetails.AddAsync(detail);
                await context.SaveChangesAsync();
            }
        }
    }
}
