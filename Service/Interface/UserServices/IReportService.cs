﻿using BusinessObjects;
using BusinessObjects.Models;

namespace Service
{
    public interface IReportService
    {
        Task<FilterListResponse<ReportDTO>> GetReports(ReportFilterDTO reportFilter);
        Task<IEnumerable<ReportDTO>> GetReportsByInfluencerId(Guid influencerId);
        Task<InfluencerReport> GetReportById(Guid id);
        Task CreateInfluencerReport(Guid influencerId, ReportRequestDTO reportRequestDTO, UserDTO userDTO);
        Task DeleteInfluencerReport(Guid id, UserDTO userDTO);
        Task ApproveReport(Guid id, UserDTO user, BannedUserRequestDTO userRequestDTO);
        Task RejectReport(Guid id);
        Task<bool> CheckIsReported(Guid influencerId, UserDTO user);
    }
}
