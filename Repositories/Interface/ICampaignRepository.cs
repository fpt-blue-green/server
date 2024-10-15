
using BusinessObjects.Models;

namespace Repositories
{
	public interface ICampaignRepository
	{
		Task<IEnumerable<Campaign>> GetAlls();
		Task<Campaign> GetById(Guid id);
		Task<List<Campaign>> GetByBrandId(Guid id);
		Task<List<Tag>> GetTagsOfCampaign(Guid campaignId);
		Task AddTagToCampaign(Guid campaignId, Guid tagId);
		Task RemoveTagOfCampaign(Guid campaignId, Guid tagId);
		Task<Campaign> GetFullDetailCampaignJobById(Guid id);
        Task Create(Campaign campaign);
		Task Update(Campaign campaign);
		Task Delete(Campaign campaign);
	}
}
