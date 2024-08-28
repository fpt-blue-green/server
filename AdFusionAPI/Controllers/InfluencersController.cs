using AutoMapper;
using BusinessObjects.DTOs;
using BusinessObjects.DTOs.InfluencerDTO;
using BusinessObjects.DTOs.InfluencerDTOs;
using BusinessObjects.Models;
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
        private List<InfluencerDTO> result = new();

        public InfluencersController(IInfluencerService influencerService, IMapper mapper)
        {
            _influencerRepository = influencerService;
            _mapper = mapper;
        }

        [HttpGet("top")]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetTopInfluencer()
        {
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
        [HttpGet("top/instagram")]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetTopInstagramInfluencer()
        {
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

        [HttpGet("top/tiktok")]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetTopTiktokInfluencer()
        {
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

        [HttpGet("top/youtube")]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetTopYoutubeInfluencer()
        {
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InfluencerDTO>>> GetExploreInfluencer([FromQuery] InfluencerFilterDTO filterDTO)
        {
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

        [HttpPost("create")]
        public async Task<IActionResult> CreateNewInfluencer([FromBody] InfluencerRequestDTO influencerRequestDTO)
        {
            var newInflu = await _influencerRepository.CreateInfluencer(influencerRequestDTO);
            return StatusCode((int)newInflu.StatusCode, newInflu);
        }

		[HttpGet("influencerTags")]
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
		public async Task<IActionResult> UpdateTagsForInfluencer([FromBody] List<Guid> listTags)
		{
			var token = Request?.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
			var result = await _influencerRepository.UpdateTagsForInfluencer(token, listTags);
			return StatusCode((int)result.StatusCode, result);

		}
	}
}