using AdFusionAPI.APIConfig;
using AutoMapper;
using BusinessObjects.DTOs.InfluencerDTOs;
using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using BusinessObjects.DTOs.UserDTOs;

namespace AdFusionAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class InfluencerController : Controller
	{
		private readonly IInfluencerService _influencerService;
		private readonly IChannelService _channelService;
		private readonly IMapper _mapper;

		public InfluencerController(IInfluencerService influencerService, IChannelService channelService, IMapper mapper)
		{
			_influencerService = influencerService;
			_channelService = channelService;
			_mapper = mapper;
		}
		[HttpPost]
		[AuthRequired]
		public async Task<IActionResult> CreateNewInfluencer([FromBody] InfluencerRequestDTO influencerRequestDTO)
		{
			var user = (UserDTO)HttpContext.Items["user"]!;
			var result = await _influencerService.CreateInfluencer(influencerRequestDTO, user);
			return Ok(result);
		}

		[HttpGet("influencerTags")]
		[AuthRequired]
		public async Task<ActionResult<List<TagDTO>>> GetTagsByInfluencer()
		{
			var user = (UserDTO)HttpContext.Items["user"]!;
			var result = await _influencerService.GetTagsByInfluencer(user);
			return Ok(result);
		}
		[HttpPost("influencerTags/update")]
		[InfluencerRequired]
		public async Task<IActionResult> UpdateTagsForInfluencer([FromBody] List<Guid> listTags)
		{
			var user = (UserDTO)HttpContext.Items["user"]!;
			var result = await _influencerService.UpdateTagsForInfluencer(user, listTags);
			return Ok(result);
		}

		[HttpPost("influencerChannels")]
		[InfluencerRequired]
		public async Task<IActionResult> CreateChannels([FromBody] Dictionary<int, string> channels)
		{
			var user = (UserDTO)HttpContext.Items["user"]!;
			await _channelService.CreateInfluencerChannel(user, channels);
			return Ok();
		}

		[HttpPut]
		[InfluencerRequired]
		public async Task<IActionResult> UpdateInfluencer([FromBody] InfluencerRequestDTO influencerRequestDTO)
		{
			var user = (UserDTO)HttpContext.Items["user"]!;
			var result = await _influencerService.UpdateInfluencer(influencerRequestDTO, user);
			return Ok(result);
		}
	}
}
