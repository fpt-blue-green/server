using BusinessObjects;
using BusinessObjects.Models;
using static BusinessObjects.JobEnumContainer;

namespace Service
{
    public interface IJobDetailService
    {
        Task UpdateJobDetailData(Job job, string? link);
        Task<FilterListResponse<CampaignJobDetailDTO>> GetCampaignJobDetail(Guid campaignId, FilterDTO filter);
        Task<CampaignJobDetailBaseDTO> GetCampaignJobDetailBaseData(Guid campaignId);
        Task<List<CampaignDailyStatsDTO>> GetCampaignDailyStats(Guid campaignId);
        Task<Dictionary<EJobStatus, int>> GetCampaignJobStatus(Guid campaignId);
    }
}
