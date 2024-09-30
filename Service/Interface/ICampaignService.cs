﻿
using BusinessObjects;
using Microsoft.AspNetCore.Http;

namespace Service
{
	public interface ICampaignService
	{

		Task<List<CampaignBrandDto>> GetCampaignsInprogres();
		Task<string> CreateCampaign(Guid userId,CampaignDTO campaign);
		Task<string> UpdateCampaign(Guid userId, CampaignDTO campaign);
		Task<List<CampaignDTO>> GetBrandCampaigns(Guid userId);
		Task<CampaignDTO> GetCampaign(Guid campaignId);
		//Task<List<TagDTO>> GetTagsOfCampaign(Guid campaignId);
		Task<string> UpdateTagsForCampaign(Guid campaignId, List<Guid> tagIds);
		Task<List<string>> UploadCampaignImages(Guid campaignId, List<Guid> imageIds, List<IFormFile> contentFiles, string folder);
    }
}
