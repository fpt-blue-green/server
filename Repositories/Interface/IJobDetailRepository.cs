using BusinessObjects.Models;

namespace Repositories
{
    public interface IJobDetailRepository
    {
        Task Create(JobDetails detail);
        Task<JobDetails> GetByDate(DateTime dateTime, Guid jobId);
    }
}
