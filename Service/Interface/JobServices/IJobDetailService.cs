using BusinessObjects;
using BusinessObjects.DTOs;
using BusinessObjects.Models;

namespace Service
{
    public interface IJobDetailService
    {
        Task UpdateJobDetailData(Job job, string? link);
        Task<FilterListResponse<CampaignJobDetailDTO>> GetCampaignJobDetail(Guid campaignId, FilterDTO filter);
        Task<CampaignJobDetailBaseDTO> GetCampaignJobDetailBaseData(Guid campaignId);
        Task<List<CampaignDailyStatsDTO>> GetCampaignDailyStats(Guid campaignId);
        Task<Dictionary<int, int>> GetCampaignJobStatus(Guid campaignId);
        Task<List<CampaignDailyStatsDTO>> GetJobDailyStats(Guid jobId, string? link);
        Task<List<JobPlatFormPieChartDTO>> GetCampaignJobDetailPlatForm(Guid campaignId);
        Task<CampaignJobDetailBaseDTO> GetJobDetailBaseData(Guid jobId, string? link);
    }
}
