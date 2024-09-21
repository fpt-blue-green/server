using AdFusionAPI.APIConfig;
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
		public async Task<ActionResult<List<CampaignBrandDto>>> GetCampaigns()
		{
			var result = await _campaignService.GetAllCampaigns();
			return Ok(result);
		}
		[HttpGet("brand")]
		[AuthRequired]
		public async Task<ActionResult<List<CampaignDTO>>> GetBrandCampaigns()
		{
			var user = (UserDTO)HttpContext.Items["user"]!;
			var result = await _campaignService.GetBrandCampaigns(user.Id);
			return Ok(result);
		}

	}
}
