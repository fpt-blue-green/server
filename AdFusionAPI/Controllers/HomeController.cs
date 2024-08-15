using AutoMapper;
using BusinessObjects.Models;
using BusinessObjects.ModelsDTO.InfluencerDTO;
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
        private readonly IMapper _mapper;
        public HomeController(IInfluencerService influencerService, IMapper mapper)
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
                var topInflue = await _influencerRepository.GetTopInfluencer();
                if (topInflue.Any())
                {
                    foreach (var item in topInflue)
                    {
                        result = _mapper.Map<List<InfluencerDTO>>(topInflue);
                    }
                }
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
                var topInflue = await _influencerRepository.GetTopInstagramInfluencer();
                if (topInflue.Any())
                {
                    foreach (var item in topInflue)
                    {
                        result = _mapper.Map<List<InfluencerDTO>>(topInflue);
                    }
                }
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
                var topInflue = await _influencerRepository.GetTopTiktokInfluencer();
                if (topInflue.Any())
                {
                    foreach (var item in topInflue)
                    {
                        result = _mapper.Map<List<InfluencerDTO>>(topInflue);
                    }
                }
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
                var topInflue = await _influencerRepository.GetTopYoutubeInfluencer();
                if (topInflue.Any())
                {
                    foreach (var item in topInflue)
                    {
                        result = _mapper.Map<List<InfluencerDTO>>(topInflue);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(result);

        }
    }
}
