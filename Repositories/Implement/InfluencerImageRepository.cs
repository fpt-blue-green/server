using BusinessObjects.Models;
using Repositories.Helper;
using Repositories.Interface;

namespace Repositories.Implement
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
