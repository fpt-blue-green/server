using BusinessObjects.Models;

namespace Repositories
{
    public interface IInfluencerImageRepository
    {
        Task Create(InfluencerImage influencerImage);
        Task<List<InfluencerImage>> GetByInfluencerId(Guid influencerId);
        Task Delete(Guid imageId);
        Task Update(InfluencerImage image);

    }
}
