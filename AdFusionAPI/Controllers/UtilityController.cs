﻿using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilityController : Controller
    {
        private readonly IUtilityService _utilityService;

        public UtilityController(IUtilityService utilityService)
        {
            this._utilityService = utilityService;
        }

        [HttpGet("location")]
        public IActionResult GetLocation(string keyName)
        {
            var cities = _utilityService.GetCitiesWithCountry(keyName);

            if (cities == null)
            {
                return NotFound();
            }

            return Ok(cities);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetChannelProfile(int platform, string channelId)
        {
            var info = await _utilityService.GetChannelProfile(platform, channelId);
            return Ok(info);
        }

        [HttpGet("video")]
        public async Task<IActionResult> GetVideoInformation(int platform, string url)
        {
            var info = await _utilityService.GetVideoInformation(platform, url);
            return Ok(info);
        }
    }
}