using BusinessObjects.Models;

namespace Repositories
{
    public interface IJobRepository
    {
        Task<IEnumerable<Job>> GetAllPedingJob();
        Task<IEnumerable<Job>> GetAllJobInProgress();
        Task<IEnumerable<string>> GetLinkJobInProgress(Guid id);
        Task<Job> GetJobInProgress(Guid id);
        Task Create(Job job);
        Task<Job> GetJobOfferById(Guid id);
        Task UpdateJobAndOffer(Job job);
        Task Update(Job job);
    }
}
