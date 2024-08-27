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
        Task<InfluencerDTO> GetInfluencerByUserId(Guid userId);
        Task<List<InfluencerDTO>> GetTopInfluencer();
        Task<List<InfluencerDTO>> GetTopInstagramInfluencer();
        Task<List<InfluencerDTO>> GetTopTiktokInfluencer();
        Task<List<InfluencerDTO>> GetTopYoutubeInfluencer();
        Task<ApiResponse<Influencer>> CreateInfluencer(InfluencerRequestDTO influencerRequestDTO, string token);
        Task<ApiResponse<Influencer>> UpdateInfluencer(InfluencerRequestDTO influencerRequestDTO, string token);
        Task UpdateInfluencer(Influencer influencer);
        Task DeleteInfluencer(Guid id);
    }
}