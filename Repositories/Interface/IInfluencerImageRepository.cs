using BusinessObjects.Models;

namespace Repositories
{
    public interface IInfluencerImageRepository
    {
        Task Create(InfluencerImage influencerImage);

    }
}
