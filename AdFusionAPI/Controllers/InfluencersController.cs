﻿using AutoMapper;
using BusinessObjects.Models;
using BusinessObjects.DTOs.InfluencerDTO;
using Microsoft.AspNetCore.Mvc;
using Repositories.Implement;
using Service.Interface;
using System.Net;

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
        [HttpGet("top")]
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
        [HttpGet("top/instagram")]
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
        [HttpGet("top/tiktok")]
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
        [HttpGet("top/youtube")]
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

        [HttpGet]
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
