using AdFusionAPI.APIConfig;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly ISystemSettingService _systemSettingService;
        private readonly ITagService _tagService;

        public AdminController(ISystemSettingService systemSettingService, ITagService tagService)
        {
            this._systemSettingService = systemSettingService;
            _tagService = tagService;
        }

        #region SystemSetting
        [HttpGet("SystemSetting")]
        [AdminRequired]
        public async Task<ActionResult<SystemSettingDTO>> GetSystemSetting(string keyName)
        {
            var result = await _systemSettingService.GetSystemSetting(keyName);
            return Ok(result);
        }

        [AdminRequired]
        [HttpPut("SystemSetting")]
        public async Task<ActionResult<string>> UpdateSystemSetting(SystemSettingDTO settingDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _systemSettingService.UpdateSystemSetting(settingDTO, user);
            return Ok(result);
        }
        #endregion

        #region Tag Management
        [AdminRequired]
        [HttpGet("Tag")]
        public async Task<ActionResult<IEnumerable<TagDetailDTO>>> GetAllTag()
        {
            var result = await _tagService.GetAllTagsWithTimeDetails();
            return Ok(result);
        }

        [AdminRequired]
        [HttpGet("Tag/{id}")]
        public async Task<ActionResult<IEnumerable<TagDetailDTO>>> GetTagById(Guid id)
        {
            var result = await _tagService.GetTagWithTimeDetailsById(id);
            return Ok(result);
        }

        [AdminRequired]
        [HttpPost("Tag")]
        public async Task<ActionResult> CreateTag(TagDTO tagDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _tagService.CreateTag(tagDTO, user);
            return Ok();
        }

        [AdminRequired]
        [HttpPut("Tag")]
        public async Task<ActionResult> UpdateTag(TagDTO tagDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _tagService.UpdateTag(tagDTO, user);
            return Ok();
        }

        [AdminRequired]
        [HttpDelete("Tag/{id}")]
        public async Task<ActionResult> DeleteTag(Guid id)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _tagService.DeleteTag(id, user);
            return Ok();
        }
        #endregion
    }
}
