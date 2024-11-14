
using BusinessObjects;
using Microsoft.AspNetCore.Http;

namespace Service
{
    public interface ICampaignService
    {
        Task<FilterListResponse<CampaignDTO>> GetCampaignsInProgress(CampaignFilterDTO filter);
        Task<Guid> CreateCampaign(Guid userId, CampaignResDto campaign);
        Task<Guid> UpdateCampaign(Guid userId, Guid CampaignId, CampaignResDto campaign);
        Task<FilterListResponse<CampaignDTO>> GetBrandCampaignsByUserId(Guid userId, BrandCampaignFilterDTO filter);
        Task<CampaignDTO> GetCampaign(Guid campaignId);
        //Task<List<TagDTO>> GetTagsOfCampaign(Guid campaignId);
        Task UpdateTagsForCampaign(Guid campaignId, List<Guid> tagIds, UserDTO userDTO);
        Task<List<string>> UploadCampaignImages(Guid campaignId, List<Guid> imageIds, List<IFormFile> contentFiles, string folder, UserDTO userDTO);
        Task DeleteCampaign(Guid campaignId, UserDTO userDTO);
        Task PublishCampaign(Guid campaignId, UserDTO userDTO);
        Task StartCampaign(Guid campaignId, UserDTO userDTO);
        Task<List<CampaignDTO>> GetAvailableBrandCampaigns(Guid brandId);
        Task<List<UserDTO>> GetCampaignParticipantInfluencer(Guid campaignId);
    }
}
