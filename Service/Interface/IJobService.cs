﻿using BusinessObjects;

namespace Service
{
    public interface IJobService
    {
        Task CreateJob(JobDTO job);
        Task BrandPaymentJob(Guid jobId, UserDTO userDto);
        Task AttachPostLink(Guid jobId, UserDTO userDTO, JobLinkDTO linkDTO);
        Task BrandCancelJob(Guid jobId, UserDTO userDTO);
    }
}
