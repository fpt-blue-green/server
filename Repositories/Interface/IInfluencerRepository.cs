
using BusinessObjects.Models;
using Pgvector;

namespace Repositories
{
    public interface IInfluencerRepository
    {
        Task<IEnumerable<Influencer>> GetAlls();
        Task<IEnumerable<Influencer>> GetInfluencerJobByCampaignId(Guid campaignId);
        Task<Influencer> GetById(Guid id);
        Task<Influencer> GetByUserId(Guid id);
        Task<Influencer> GetBySlug(string slug);
        Task<List<Tag>> GetTagsByInfluencer(Guid influencerId);
        Task<Influencer> GetInfluencerWithFeedbackById(Guid id);
        Task AddTagToInfluencer(Guid influencerId, Guid tagId);
		Task RemoveTagOfInfluencer(Guid influencerId, Guid tagId);
		Task Create(Influencer influencer);
        Task Update(Influencer influencer);
        Task Delete(Guid id);
        Task<IEnumerable<Influencer>> GetSimilarInfluencers(Vector embedding, int pageSize, int page);
        Task<Influencer> GetInfluencerWithEmbeddingById(Guid id);
    }
}
