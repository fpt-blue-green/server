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
        [HttpGet("topInfluencer")]
		public async Task<ActionResult<IEnumerable<Influencer>>> GetTopInfluencer()
		{
			try
			{
                var topInflue = await _influencerRepository.GetTopInfluencer();
                return Ok(topInflue);

            }catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
        }
		[HttpGet("topInfluencerInstagram")]
		public async Task<ActionResult<IEnumerable<Influencer>>> GetTopInstagramInfluencer()
		{
			try
			{
                var topInflue = await _influencerRepository.GetTopInstagramInfluencer();
                return Ok(topInflue);
            }catch(Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpGet("topInfluencerTiktok")]
		public async Task<ActionResult<IEnumerable<Influencer>>> GetTopTiktokInfluencer()
		{
			try
			{
                var topInflue = await _influencerRepository.GetTopTiktokInfluencer();
                return Ok(topInflue);
            }catch( Exception ex) { 
				return BadRequest(ex.Message);
            }
        }
		[HttpGet("topInfluencerYoutube")]
		public async Task<ActionResult<IEnumerable<Influencer>>> GetTopYoutubeInfluencer()
		{
			try
			{
                var topInflue = await _influencerRepository.GetTopYoutubeInfluencer();
                return Ok(topInflue);
            }catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
			
		}
	}
}
