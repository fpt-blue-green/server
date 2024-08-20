using AutoMapper;
using BusinessObjects.ModelsDTO.InfluencerDTO;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfluencersController : Controller
    {
        private readonly IInfluencerService _influencerRepository;
        private readonly IMapper _mapper;
        public InfluencersController(IInfluencerService influencerService, IMapper mapper)
        {
            _influencerRepository = influencerService;
            _mapper = mapper;
        }
        [HttpGet("topInfluencer")]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetTopInfluencer()
        {
            var result = new List<InfluencerDTO>();
            try
            {
                result = await _influencerRepository.GetTopInfluencer();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(result);
        }
        [HttpGet("topInfluencerInstagram")]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetTopInstagramInfluencer()
        {
            var result = new List<InfluencerDTO>();
            try
            {
                result = await _influencerRepository.GetTopInstagramInfluencer();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(result);
        }
        [HttpGet("topInfluencerTiktok")]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetTopTiktokInfluencer()
        {
            var result = new List<InfluencerDTO>();
            try
            {
                result = await _influencerRepository.GetTopTiktokInfluencer();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(result);
        }
        [HttpGet("topInfluencerYoutube")]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetTopYoutubeInfluencer()
        {
            var result = new List<InfluencerDTO>();
            try
            {
                result = await _influencerRepository.GetTopYoutubeInfluencer();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(result);

        }

        [HttpGet("paging/influencers")]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetExploreInfluencer([FromQuery] InfluencerFilterDTO filterDTO)
        {
            var result = new List<InfluencerDTO>();
            try
            {
                result = await _influencerRepository.GetAllInfluencers(filterDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(result);
        }
    }
}
