using AdFusionAPI.APIConfig;
using BusinessObjects;
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

        [HttpGet]
        [AdminRequired]
        public async Task<ActionResult<SystemSettingDTO>> Get(string keyName)
        {
            var result = await systemSettingService.GetSystemSetting(keyName);
            return Ok(result);
        }

        [AdminRequired]
        [HttpPut]
        public async Task<ActionResult<string>> Update(SystemSettingDTO settingDTO)
        {
            var result = await systemSettingService.UpdateSystemSetting(settingDTO);
            return Ok(result);

        }
    }
}
