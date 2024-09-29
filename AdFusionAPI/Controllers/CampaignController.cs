using AdFusionAPI.APIConfig;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CampaignController : Controller
	{
		private readonly ICampaignService _campaignService;
		private readonly ICampaignContentService _campaignContentService;


		public CampaignController(ICampaignService campaignService, ICampaignContentService campaignContentService)
		{
			_campaignService = campaignService;
			_campaignContentService = campaignContentService;
		}


		[HttpGet("{id}")]
		[AuthRequired]
		public async Task<ActionResult<CampaignDTO>> GetCampaign(Guid id)
		{
			var result = await _campaignService.GetCampaign(id);
			return Ok(result);
		}

		[HttpPost("")]
		[AuthRequired]
		public async Task<ActionResult<string>> CreateCampaign([FromBody] CampaignDTO campaign)
		{
			var user = (UserDTO)HttpContext.Items["user"]!;
			var result = await _campaignService.CreateCampaign(user.Id, campaign);
			return Ok(result);
		}

		[HttpPut]
		[AuthRequired]
		public async Task<ActionResult<string>> UpdateCampaign([FromBody] CampaignDTO campaign)
		{
			var user = (UserDTO)HttpContext.Items["user"]!;
			var result = await _campaignService.UpdateCampaign(user.Id, campaign);
			return Ok(result);
		}
		[HttpGet("tags")]
		[AuthRequired]
		public async Task<ActionResult<List<TagDTO>>> GetTagsOfCampaign( Guid campaignId)
		{
			var result = await _campaignService.GetTagsOfCampaign(campaignId);
			return Ok(result);
		}

		[HttpPost("tags")]
		[AuthRequired]
		public async Task<ActionResult<string>> UpdateTagsOfCampaign( Guid campaignId, List<Guid> listTags)
		{
			var result = await _campaignService.UpdateTagsForCampaign(campaignId, listTags);
			return Ok(result);
		}

        [HttpPost("images")]
        [AuthRequired]
        public async Task<ActionResult<List<string>>> UploadImages([FromForm] List<Guid> imageIds, [FromForm] List<IFormFile> images,Guid campaginId)
        {
            var result = await _campaignService.UploadCampaignImages(campaginId, imageIds, images, "CampaignImages");
            return Ok(result);
        }

        #region content
        [HttpPost("contents")]
		[AuthRequired]
		public async Task<ActionResult<string>> CreateCampaignContents([FromBody] List<CampaignContentDto> contents, Guid campaignId)
		{
			var result = await _campaignContentService.CreateCampaignContents( campaignId, contents);
			return Ok(result);
		}
		/*[HttpPut("contents/{contentId}")]
		[AuthRequired]
		public async Task<ActionResult<string>> UpdateCampaignContent([FromBody] CampaignContentResDto content, Guid contentId,Guid campaignId)
		{
			var result = await _campaignContentService.UpdateCampaignContent(campaignId, contentId, content);
			return Ok(result);
		}*/
		[HttpGet("contents")]
		[AuthRequired]
		public async Task<ActionResult<List<InfluencerDTO>>> GetCampaignContents(Guid campaignId)
		{
			var result = await _campaignContentService.GetCampaignContents(campaignId);
			return Ok(result);
		}
		/*[HttpGet("contents/{contentId}")]
		[AuthRequired]
		public async Task<ActionResult<PackageDTO>> GetCampaignContent(Guid contentId)
		{
			var result = await _campaignContentService.GetCampaignContent(contentId);
			return Ok(result);
		}*/
		#endregion
	}
}
