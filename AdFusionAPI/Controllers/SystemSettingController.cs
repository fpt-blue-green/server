using BusinessObjects.ModelsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace AdFusionAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SystemSettingController : Controller
    {
        private readonly ISystemSettingService systemSettingService;

        public SystemSettingController(ISystemSettingService systemSettingService)
        {
            this.systemSettingService = systemSettingService;
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("getSystemSetting/{keyName}")]
        public async Task<IActionResult> Get(string keyName)
        {
            var result = await systemSettingService.GetSystemSetting(keyName);
            return StatusCode((int)result.StatusCode, result);
        }


        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("updateSystemSetting")]
        public async Task<IActionResult> Update(SystemSettingDTO settingDTO)
        {
            var result = await systemSettingService.UpdateSystemSetting(settingDTO);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
