using BusinessObjects.Models;

namespace Repositories
{
    public interface IInfluencerImageRepository
    {
        Task Create(InfluencerImage influencerImage);
        Task<List<InfluencerImage>> GetByInfluencerId(Guid influencerId);
        Task DeleteByUrl(string url);
        Task Delete(Guid imageId);
        Task<List<InfluencerImage>> GetByIds(List<string> imageIds);
        Task Update(InfluencerImage image);
        Task<int> GetImagesCountByInfluencerId(Guid influencerId);

    }
}
