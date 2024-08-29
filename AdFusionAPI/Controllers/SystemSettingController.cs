using AdFusionAPI.APIConfig;
using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface.SystemServices;

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
            return StatusCode((int)result.StatusCode, result);
        }

        [AdminRequired]
        [Authorize(Policy = "AdminPolicy")]
        [HttpPut]
        public async Task<IActionResult> Update(SystemSettingDTO settingDTO)
        {
            var result = await systemSettingService.UpdateSystemSetting(settingDTO);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
