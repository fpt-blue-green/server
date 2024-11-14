using BusinessObjects;

namespace Service
{
    public interface IJobService
    {
        Task CreateJob(JobRequestDTO job);
        Task BrandPaymentJob(Guid jobId, UserDTO userDto);
        Task AttachPostLink(Guid jobId, UserDTO userDTO, JobLinkDTO linkDTO);
        Task BrandCancelJob(Guid jobId, UserDTO userDTO);
        Task BrandCompleteJob(Guid jobId, UserDTO userDto);
        Task BrandFaliedJob(Guid jobId, UserDTO userDto);
        Task<FilterListResponse<JobDTO>> GetAllJobByCurrentAccount(UserDTO user, JobFilterDTO filter);
        Task<List<JobStatistical>> Statistical(UserDTO user);
        Task<List<string>> GetJobLink(Guid jobId);
    }
}
