
using BusinessObjects;
using Microsoft.AspNetCore.Http;

namespace Service
{
	public interface ICampaignService
	{

		Task<List<CampaignDTO>> GetCampaignsInprogres(CampaignFilterDto filter);
		Task<Guid> CreateCampaign(Guid userId,CampaignResDto campaign);
		Task<Guid> UpdateCampaign(Guid userId,Guid CampaignId, CampaignResDto campaign);
		Task<List<CampaignDTO>> GetBrandCampaigns(Guid userId);
		Task<CampaignDTO> GetCampaign(Guid campaignId);
		//Task<List<TagDTO>> GetTagsOfCampaign(Guid campaignId);
		Task UpdateTagsForCampaign(Guid campaignId, List<Guid> tagIds);
		Task<List<string>> UploadCampaignImages(Guid campaignId, List<Guid> imageIds, List<IFormFile> contentFiles, string folder);
		Task DeleteCampaign(Guid campaignId);

	}
}
