﻿using BusinessObjects.Models;

namespace Repositories
{
    public interface IJobDetailRepository
    {
        Task Create(JobDetails detail);
        Task<(int totalViews, int totalLikes, int totalComments)> GetTotalMetricsByLinkAndJobId(string link, Guid jobId);
        Task<JobDetails> GetByLinkAndJobId(string link, Guid jobId);
        Task<IEnumerable<JobDetails>> GetLinkByJobId(Guid jobId);
    }
}
