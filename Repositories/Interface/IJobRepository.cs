﻿using BusinessObjects.Models;

namespace Repositories
{
    public interface IJobRepository
    {
        Task<IEnumerable<Job>> GetAllJob();
        Task<IEnumerable<Job>> GetAllPedingJob();
        Task<IEnumerable<Job>> GetAllJobInProgress();
        Task<IEnumerable<string>> GetLinkJobAppovedInProgress(Guid id);
        Task<IEnumerable<Job>> GetCampaignJobs(Guid campaginId);
        Task<Job> GetJobInProgress(Guid id);
        Task Create(Job job);
        Task<Job> GetJobFullDetailById(Guid id);
        Task UpdateJobAndOffer(Job job);
        Task Update(Job job);
        Task<IEnumerable<Job>> GetJobInfluencerByUserId(Guid userId);
        Task<IEnumerable<Job>> GetJobBrandByUserId(Guid userId);
        Task<IEnumerable<Job>> GetAllJobDone();
        Task<IEnumerable<Job>> GetAllJobIgnoreFilter();
    }
}
