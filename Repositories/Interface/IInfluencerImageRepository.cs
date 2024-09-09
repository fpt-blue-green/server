using BusinessObjects.Models;

namespace Repositories
{
    public interface IInfluencerImageRepository
    {
        Task Create(InfluencerImage influencerImage);
        Task<List<InfluencerImage>> GetByInfluencerId(Guid influencerId);
        Task DeleteByUrl(string url);
        Task Delete(Guid imageId);

        Task Update(InfluencerImage image);
        Task<int> GetCountByInfluencerId(Guid influencerId);

    }
}
