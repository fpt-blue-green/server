using AdFusionAPI.APIConfig;
using AutoMapper;
using BusinessObjects.DTOs;
using BusinessObjects.DTOs.InfluencerDTO;
using BusinessObjects.DTOs.InfluencerDTOs;
using BusinessObjects.DTOs.UserDTOs;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfluencersController : Controller
    {
        private readonly IInfluencerService _influencerRepository;
        private readonly IChannelService _channelService;
        private readonly IMapper _mapper;

        public InfluencersController(IInfluencerService influencerService,IChannelService channelService, IMapper mapper)
        {
            _influencerRepository = influencerService;
            _channelService = channelService;
            _mapper = mapper;
        }

        [HttpGet("top")]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetTopInfluencer()
        {
            var result = await _influencerRepository.GetTopInfluencer();
            return Ok(result);
        }

        [HttpGet("top/instagram")]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetTopInstagramInfluencer()
        {
            var result = await _influencerRepository.GetTopInstagramInfluencer();
            return Ok(result);
        }

        [HttpGet("top/tiktok")]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetTopTiktokInfluencer()
        {
            var result = await _influencerRepository.GetTopTiktokInfluencer();
            return Ok(result);
        }

        [HttpGet("top/youtube")]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetTopYoutubeInfluencer()
        {
            var result = await _influencerRepository.GetTopYoutubeInfluencer();
            return Ok(result);

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetExploreInfluencer([FromQuery] InfluencerFilterDTO filterDTO)
        {
            var result = await _influencerRepository.GetAllInfluencers(filterDTO);
            return Ok(result);
        }

        [HttpPost]
        [InfluencerRequired]
        public async Task<IActionResult> CreateNewInfluencer([FromBody] InfluencerRequestDTO influencerRequestDTO)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var newInflu = await _influencerRepository.CreateInfluencer(influencerRequestDTO, token);
            return StatusCode((int)newInflu.StatusCode, newInflu);
        }

        [HttpGet("influencerTags")]
        [InfluencerRequired]
        public async Task<ActionResult<List<TagDTO>>> GetTagsByInfluencer()
        {
            var token = Request?.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
            var result = await _influencerRepository.GetTagsByInfluencer(token);
            return StatusCode((int)result.StatusCode, result);
        }


        /*[HttpPost("influencerTags/add")]
		public async Task<IActionResult> CreateNewInfluencer([FromBody] List<Guid> listTags)
		{
			var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
			var result = await _influencerRepository.AddTagToInfluencer(token, listTags);
			return StatusCode((int)result.StatusCode, result);
		}
*/
        [HttpPost("influencerTags/update")]
        [InfluencerRequired]
        public async Task<IActionResult> UpdateTagsForInfluencer([FromBody] List<Guid> listTags)
        {
            var token = Request?.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
            var result = await _influencerRepository.UpdateTagsForInfluencer(token!, listTags);
            return StatusCode((int)result.StatusCode, result);

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
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var influencer = await _influencerRepository.UpdateInfluencer(influencerRequestDTO, token);
            return StatusCode((int)influencer.StatusCode, influencer);
        }
    }
}