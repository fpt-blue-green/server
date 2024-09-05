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
        private readonly IInfluencerService _influencerService;
        private readonly IChannelService _channelService;
        private readonly IMapper _mapper;

        public InfluencersController(IInfluencerService influencerService,IChannelService channelService, IMapper mapper)
        {
            _influencerService = influencerService;
            _channelService = channelService;
            _mapper = mapper;
        }

        [HttpGet("top")]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetTopInfluencer()
        {
            var result = await _influencerService.GetTopInfluencer();
            return Ok(result);
        }

        [HttpGet("top/instagram")]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetTopInstagramInfluencer()
        {
            var result = await _influencerService.GetTopInstagramInfluencer();
            return Ok(result);
        }

        [HttpGet("top/tiktok")]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetTopTiktokInfluencer()
        {
            var result = await _influencerService.GetTopTiktokInfluencer();
            return Ok(result);
        }

        [HttpGet("top/youtube")]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetTopYoutubeInfluencer()
        {
            var result = await _influencerService.GetTopYoutubeInfluencer();
            return Ok(result);

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetExploreInfluencer([FromQuery] InfluencerFilterDTO filterDTO)
        {
            var result = await _influencerService.GetAllInfluencers(filterDTO);
            return Ok(result);
        }
    }
}