using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories.Implement;
using Service.Interface;
using System.Net;

namespace AdFusionAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class HomeController : Controller
	{
		private readonly IInfluencerService _influencerRepository;
        public HomeController(IInfluencerService influencerService)
        {
			_influencerRepository = influencerService;

		}
        [HttpGet("home/topInfluencer")]
		public async Task<ActionResult<IEnumerable<Influencer>>> GetTopInfluencer()
		{
			var topInflue = await _influencerRepository.GetTopInfluencer();
			return Ok(topInflue);
		}
		[HttpGet("home/topInfluencerInstagram")]
		public async Task<ActionResult<IEnumerable<Influencer>>> GetTopInstagramInfluencer()
		{
			var topInflue = await _influencerRepository.GetTopInstagramInfluencer();
			return Ok(topInflue);
		}
		[HttpGet("home/topInfluencerTiktok")]
		public async Task<ActionResult<IEnumerable<Influencer>>> GetTopTiktokInfluencer()
		{
			var topInflue = await _influencerRepository.GetTopTiktokInfluencer();
			return Ok(topInflue);
		}
		[HttpGet("home/topInfluencerYoutube")]
		public async Task<ActionResult<IEnumerable<Influencer>>> GetTopYoutubeInfluencer()
		{
			var topInflue = await _influencerRepository.GetTopYoutubeInfluencer();
			return Ok(topInflue);
		}
	}
}
