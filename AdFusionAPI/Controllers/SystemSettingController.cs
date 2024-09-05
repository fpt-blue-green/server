using AdFusionAPI.APIConfig;
using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemSettingController : Controller
    {
        private readonly ISystemSettingService systemSettingService;

        public SystemSettingController(ISystemSettingService systemSettingService)
        {
            this.systemSettingService = systemSettingService;
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        [AdminRequired]
        public async Task<IActionResult> Get(string keyName)
        {
            var result = await systemSettingService.GetSystemSetting(keyName);
            return Ok(result);
        }

        [AdminRequired]
        [Authorize(Policy = "AdminPolicy")]
        [HttpPut]
        public async Task<IActionResult> Update(SystemSettingDTO settingDTO)
        {
            var result = await systemSettingService.UpdateSystemSetting(settingDTO);
            return Ok(result);

        }
    }
}
