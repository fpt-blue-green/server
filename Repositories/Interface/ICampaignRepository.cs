
using BusinessObjects.Models;

namespace Repositories
{
	public interface ICampaignRepository
	{
		Task<IEnumerable<Campaign>> GetAlls();
		Task<Campaign> GetById(Guid id);
		Task<IEnumerable<Campaign>> GetByBrandIdId(Guid id);
		Task Create(Campaign campaign);
		Task Update(Campaign campaign);
		Task Delete(Guid id);
	}
}
