using BusinessObjects.Models;

namespace Repositories
{
    public interface ICampaignImageRepository
    {
        Task Create(CampaignImage campaignImage);
        Task<List<CampaignImage>> GetByCampaignId(Guid campaignId);
        Task Delete(Guid imageId);
        Task Update(CampaignImage image);
        Task<int> GetImagesCountByCampaignId(Guid campaignId);
    }
}
