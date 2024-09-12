﻿using BusinessObjects;

namespace Service
{
    public interface IInfluencerService
    {
        Task<List<InfluencerDTO>> GetAllInfluencers();
        Task<GetInfluencersResponseDTO> GetAllInfluencers(InfluencerFilterDTO filter);
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
        Task<string> ValidatePhoneNumber(UserDTO user, string phoneNumber);
        Task DeleteInfluencer(Guid id);
    }
}