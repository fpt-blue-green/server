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
        private readonly ISystemSettingService _systemSettingService;

        public SystemSettingController(ISystemSettingService systemSettingService)
        {
            this._systemSettingService = systemSettingService;
        }

        #region SystemSetting
        [HttpGet()]
        [AdminRequired]
        public async Task<ActionResult<IEnumerable<SystemSettingDTO>>> GetSystemSettings()
        {
            var result = await _systemSettingService.GetSystemSettings();
            return Ok(result);
        }

        [HttpGet("{keyName}")]
        [AdminRequired]
        public async Task<ActionResult<SystemSettingDTO>> GetSystemSetting(string keyName)
        {
            var result = await _systemSettingService.GetSystemSetting(keyName);
            return Ok(result);
        }

        [AdminRequired]
        [HttpPut()]
        public async Task<ActionResult> UpdateSystemSetting(SystemSettingDTO settingDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _systemSettingService.UpdateSystemSetting(settingDTO, user);
            return Ok();
        }
        #endregion

     
    }
}
