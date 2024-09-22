﻿
using BusinessObjects.Models;

namespace Repositories
{
	public interface ICampaignRepository
	{
		Task<IEnumerable<Campaign>> GetAlls();
		Task<Campaign> GetById(Guid id);
		Task<IEnumerable<Campaign>> GetByBrandIdId(Guid id);
		Task<List<Tag>> GetTagsOfCampaign(Guid campaignId);
		Task AddTagToCampaign(Guid campaignId, Guid tagId);
		Task RemoveTagOfCampaign(Guid campaignId, Guid tagId);
		Task Create(Campaign campaign);
		Task Update(Campaign campaign);
		Task Delete(Guid id);
	}
}