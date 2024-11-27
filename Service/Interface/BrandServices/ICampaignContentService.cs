

using BusinessObjects;

namespace Service
{
    public interface ICampaignContentService
    {
        Task CreateCampaignContents(Guid campaignId, List<CampaignContentDto> campaignContents, UserDTO userDTO);
        //Task<string> UpdateCampaignContent(Guid campaignId, Guid campaignContentId, CampaignContentResDto campaignContentDto);
        Task<List<CampaignContentDto>> GetCampaignContents(Guid campaignId, UserDTO userDTO);
        Task<CampaignContentDto> GetCampaignContent(Guid campaignContentId);
    }
}
