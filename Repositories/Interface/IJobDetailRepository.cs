using BusinessObjects.Models;

namespace Repositories
{
    public interface IJobDetailRepository
    {
        Task Create(JobDetail detail);
    }
}
