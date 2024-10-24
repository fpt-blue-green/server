using BusinessObjects;

namespace Service
{
    public interface IJobService
    {
        Task CreateJob(JobRequestDTO job);
        Task BrandPaymentJob(Guid jobId, UserDTO userDto);
        Task AttachPostLink(Guid jobId, UserDTO userDTO, JobLinkDTO linkDTO);
        Task BrandCancelJob(Guid jobId, UserDTO userDTO);
        Task<JobResponseDTO> GetAllJobByCurrentAccount(UserDTO user, JobFilterDto filter);
        Task<List<JobStatistical>> Statistical(UserDTO user);
    }
}
