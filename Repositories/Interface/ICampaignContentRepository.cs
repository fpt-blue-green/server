

using BusinessObjects.Models;

namespace Repositories
{
	public interface ICampaignContentRepository
	{
		Task<IEnumerable<CampaignContent>> GetAlls();
		Task<CampaignContent> GetById(Guid id);
		Task CreateList(List<CampaignContent> campaignContents);
		Task Update(CampaignContent campaignContent);
		Task Delete(Guid id);
	}
}
