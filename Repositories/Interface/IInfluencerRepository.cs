using BusinessObjects.DTOs.InfluencerDTOs;
using BusinessObjects.Models;

namespace Repositories.Interface
{
    public interface IInfluencerRepository
    {
        Task<IEnumerable<Influencer>> GetAlls();
        Task<Influencer> GetById(Guid id);
<<<<<<< HEAD
        Task<Influencer> GetByUserId(Guid id);
        Task<List<Tag>> GetTagsByInfluencer(Guid influencerId);
        //Task AddTagToInfluencer(Guid influencerId, Guid tagId);
        //Task RemoveTagFromInfluencer(Guid influencerId, Guid tagId);
        Task UpdateTagsForInfluencer(Guid influencerId, List<Guid> tagIds);
=======
        Task<Influencer> GetByUserId(Guid userId);
>>>>>>> 6a03d7a ([ADF-94][ADF-97] update code)
        Task Create(Influencer influencer);
        Task Update(Influencer influencer);
        Task Delete(Guid id);
    }
}
