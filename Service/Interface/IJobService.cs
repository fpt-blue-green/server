using BusinessObjects;

namespace Service
{
    public interface IJobService
    {
        Task CreateJob(JobDTO job);
        Task BrandPaymentJob(Guid jobId, UserDTO userDto);
        Task BrandCancleJob(Guid jobId);
    }
}
