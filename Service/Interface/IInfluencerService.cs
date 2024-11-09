using BusinessObjects;
using Microsoft.AspNetCore.Http;

namespace Service
{
    public interface IInfluencerService
    {
        Task<List<InfluencerDTO>> GetAllInfluencers();
        Task<FilterListResponse<InfluencerDTO>> GetAllInfluencers(InfluencerFilterDTO filter);
        Task<InfluencerDTO> GetInfluencerById(Guid id);
        Task<InfluencerDTO> GetInfluencerBySlug(string slug);
        Task<List<TagDTO>> GetTagsByInfluencer(UserDTO user);
        Task<string> UpdateTagsForInfluencer(UserDTO user, List<Guid> tagIds);
        Task<InfluencerDTO> GetInfluencerByUserId(Guid userId);
        Task<List<InfluencerDTO>> GetTopInfluencer();
        Task<List<InfluencerDTO>> GetTopInstagramInfluencer();
        Task<List<InfluencerDTO>> GetTopTiktokInfluencer();
        Task<List<InfluencerDTO>> GetTopYoutubeInfluencer();
        Task<string> CreateOrUpdateInfluencer(InfluencerRequestDTO influencerRequestDTO, UserDTO user);
        Task<bool> SendPhoneOtp(string phone);
        Task<bool> VerifyPhoneOtp(UserDTO user, string phone, string otp);
        Task DeleteInfluencer(Guid id);
        Task<List<string>> UploadContentImages(List<Guid> imageIds, List<IFormFile> contentFiles, UserDTO user, string folder);
        Task<FilterListResponse<InfluencerJobDTO>> GetInfluencerWithJobByCampaginId(Guid campaignId, InfluencerJobFilterDTO filter, UserDTO user);
    }
}