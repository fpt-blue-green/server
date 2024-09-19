
using BusinessObjects.Models;

namespace Repositories
{
    public interface IInfluencerRepository
    {
        Task<IEnumerable<Influencer>> GetAlls();
        Task<Influencer> GetById(Guid id);
        Task<Influencer> GetByUserId(Guid id);
        Task<Influencer> GetBySlug(string slug);
        Task<List<Tag>> GetTagsByInfluencer(Guid influencerId);
        Task<Influencer> GetInfluencerByFeedbackID(Guid feedbackId);
        Task AddTagToInfluencer(Guid influencerId, Guid tagId);
		Task RemoveTagOfInfluencer(Guid influencerId, Guid tagId);
		Task Create(Influencer influencer);
        Task Update(Influencer influencer);
        Task Delete(Guid id);
    }
}
