using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace AdFusionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilityController : Controller
    {
        private readonly IUtilityService utilityService;

        public UtilityController(IUtilityService utilityService)
        {
            this.utilityService = utilityService;
        }

        [HttpGet("location/{keyName}")]
        public async Task<IActionResult> GetLocation(string keyName)
        {
            var cities = utilityService.GetCitiesWithCountry(keyName);

            if (cities == null)
            {
                return NotFound();
            }
            return Ok(cities);

        }

    }
}
