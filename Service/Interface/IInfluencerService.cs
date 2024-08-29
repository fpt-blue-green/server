using BusinessObjects.DTOs;
using BusinessObjects.DTOs.InfluencerDTO;
using BusinessObjects.DTOs.InfluencerDTOs;
using BusinessObjects.Models;

namespace Service.Interface
{
    public interface IInfluencerService
    {
        Task<List<InfluencerDTO>> GetAllInfluencers();
        Task<List<InfluencerDTO>> GetAllInfluencers(InfluencerFilterDTO filter);
        Task<InfluencerDTO> GetInfluencerById(Guid id);
        Task<ApiResponse<List<TagDTO>>> GetTagsByInfluencer(string token);
        //Task<ApiResponse<object>> AddTagToInfluencer(string token, List<Guid> tagIds);
        Task<ApiResponse<object>> UpdateTagsForInfluencer(string token, List<Guid> tagIds);
        Task<InfluencerDTO> GetInfluencerByUserId(Guid userId);

        Task<List<InfluencerDTO>> GetTopInfluencer();
        Task<List<InfluencerDTO>> GetTopInstagramInfluencer();
        Task<List<InfluencerDTO>> GetTopTiktokInfluencer();
        Task<List<InfluencerDTO>> GetTopYoutubeInfluencer();
        Task<ApiResponse<Influencer>> CreateInfluencer(InfluencerRequestDTO influencerRequestDTO, string token);
        Task<ApiResponse<Influencer>> UpdateInfluencer(InfluencerRequestDTO influencerRequestDTO, string token);
        Task DeleteInfluencer(Guid id);
    }
}