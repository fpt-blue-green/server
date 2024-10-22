using BusinessObjects.Models;

namespace Repositories
{
    public interface IJobRepository
    {
        Task<IEnumerable<Job>> GetAllPedingJob();
        Task<IEnumerable<Job>> GetAllJobInProgress();
        Task<IEnumerable<string>> GetLinkJobInProgress(Guid id);
        Task<IEnumerable<Job>> GetCampaignJobs(Guid campaginId);
        Task<Job> GetJobInProgress(Guid id);
        Task Create(Job job);
        Task<Job> GetJobFullDetailById(Guid id);
        Task UpdateJobAndOffer(Job job);
        Task Update(Job job);
        Task<IEnumerable<Job>> GetJobInfluencerByUserId(Guid userId);
        Task<IEnumerable<Job>> GetJobBrandByUserId(Guid userId);
        Task<IEnumerable<Job>> FilterJobBrandByJobStatus(Guid userId, int eJobStatus);
        Task<IEnumerable<Job>> FilterJobInfluencerByJobStatus(Guid userId, int eJobStatus);
        Task<IEnumerable<Job>> FilterJobInfluencerByCampaignStatus(Guid userId, int eCampaignStatus);
        Task<IEnumerable<Job>> FilterJobBrandByCampaignStatus(Guid userId, int eCampaignStatus);


    }
}
