using BusinessObjects.Models;

namespace Repositories
{
    public interface IJobRepository
    {
        Task<IEnumerable<Job>> GetAllPedingJob();
        Task Create(Job job);
        Task<Job> GetJobOfferById(Guid id);
        Task UpdateJobAndOffer(Job job);
    }
}
