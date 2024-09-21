using AdFusionAPI.APIConfig;
using AutoMapper;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CampaignsController : Controller
	{
        private readonly ICampaignService _campaignService;

		public CampaignsController(ICampaignService campaignService)
		{
			_campaignService = campaignService;
		}
		[HttpGet()]
		[AuthRequired]
		public async Task<ActionResult<List<CampaignDTO>>> GetBrandCampaigns()
		{
			var user = (UserDTO)HttpContext.Items["user"]!;
			var result = await _campaignService.GetBrandCampaigns(user.Id);
			return Ok(result);
		}

		[HttpGet("{id}")]
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
	}
}
