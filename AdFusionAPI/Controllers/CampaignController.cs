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

		public CampaignController(ICampaignService campaignService)
		{
			_campaignService = campaignService;
		}
		

		[HttpGet("{id}")]
		[AuthRequired]
		public async Task<ActionResult<CampaignDTO>> GetBrandCampaign(Guid id)
		{
			var user = (UserDTO)HttpContext.Items["user"]!;
			var result = await _campaignService.GetBrandCampaign(user.Id, id);
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
	}
}
