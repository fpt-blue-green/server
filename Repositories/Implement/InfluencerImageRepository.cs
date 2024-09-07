using BusinessObjects.Models;
namespace Repositories
{
    public class InfluencerImageRepository : SingletonBase<InfluencerImageRepository>, IInfluencerImageRepository
    {
        public InfluencerImageRepository() { }

        public async Task Create(InfluencerImage influencerImage)
        {
            await context.InfluencerImages.AddAsync(influencerImage);
            await context.SaveChangesAsync();
        }
    }
}
