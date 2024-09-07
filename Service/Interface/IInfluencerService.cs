using BusinessObjects;

namespace Service
{
    public interface IInfluencerService
    {
        Task<List<InfluencerDTO>> GetAllInfluencers();
        Task<List<InfluencerDTO>> GetAllInfluencers(InfluencerFilterDTO filter);
        Task<InfluencerDTO> GetInfluencerById(Guid id);
        Task<List<TagDTO>> GetTagsByInfluencer(UserDTO user);
        Task<string> UpdateTagsForInfluencer(UserDTO user, List<Guid> tagIds);
        Task<InfluencerDTO> GetInfluencerByUserId(Guid userId);

        Task<List<InfluencerDTO>> GetTopInfluencer();
        Task<List<InfluencerDTO>> GetTopInstagramInfluencer();
        Task<List<InfluencerDTO>> GetTopTiktokInfluencer();
        Task<List<InfluencerDTO>> GetTopYoutubeInfluencer();
        Task<string> CreateOrUpdateInfluencer(InfluencerRequestDTO influencerRequestDTO, UserDTO user);
        Task DeleteInfluencer(Guid id);
    }
}