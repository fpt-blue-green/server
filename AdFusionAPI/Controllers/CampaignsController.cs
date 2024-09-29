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
		public async Task<ActionResult<List<CampaignBrandDto>>> GetCampaignsInprogres()
		{
			var result = await _campaignService.GetCampaignsInprogres();
			return Ok(result);
		}
		

	}
}
