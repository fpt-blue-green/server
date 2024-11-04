using BusinessObjects.Models;

namespace Repositories
{
    public interface IJobDetailRepository
    {
        Task Create(JobDetails detail);
        Task<(int totalViews, int totalLikes, int totalComments)> GetTotalMetricsByLinkAndJobId(string link, Guid jobId);
    }
}
