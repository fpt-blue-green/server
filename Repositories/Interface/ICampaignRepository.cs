﻿
using BusinessObjects.Models;
using Pgvector;

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
		Task<Campaign> GetCampaignJobDetails(Guid campaignId);
		Task<Campaign> GetByCampaignChatId(Guid campaignChatId);
		Task<IEnumerable<Campaign>> GetAllsIgnoreFilter();
		Task<Campaign> GetCampaignAllJobDetails(Guid campaignId);
		Task<List<User>> GetInfluencerParticipant(Guid campaignId);
		Task<List<JobDetails>> GetDailyJobStatus(Guid jobId, string link);
		Task<List<JobDetails>> GetAllDailyJobStatus(Guid jobId);
		Task<List<Campaign>> GetSimilarCampaigns(Vector embedding);
	}
}
