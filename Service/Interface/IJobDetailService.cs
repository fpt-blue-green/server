using BusinessObjects.Models;

namespace Service
{
    public interface IJobDetailService
    {
        Task UpdateJobDetailData(Job job, string? link);
    }
}
