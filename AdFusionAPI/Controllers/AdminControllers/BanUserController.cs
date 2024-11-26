using AdFusionAPI.APIConfig;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers.AdminControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BanUserController : Controller
    {
        private readonly IBannedUserService _banService;
        public BanUserController(IBannedUserService banService)
        {
            _banService = banService;
        }

        [HttpPost("{id}/unBan")]
        [AdminRequired]
        public async Task<ActionResult> UnBanUser(Guid id, [FromBody] BannedUserRequestDTO userRequestDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _banService.UnBanUser(id, userRequestDTO, user);
            return Ok();
        }

        [HttpPost("{id}/ban")]
        [AdminRequired]
        public async Task<ActionResult> BanUser(Guid id, [FromBody] BannedUserRequestDTO userRequestDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            await _banService.BanUser(id, userRequestDTO, user);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BannedUserDTO>>> GetBannedUserData([FromQuery] FilterDTO filter)
        {
            var result = await _banService.GetBannedUsers(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BannedUserDTO>> GetBannedUserData(Guid id)
        {
            var result = await _banService.GetBannedUserById(id);
            return Ok(result);
        }

        [HttpGet("export")]
        [AdminRequired]
        public async Task<IActionResult> DownloadDataFile()
        {
            var fileContent = await _banService.GetDataFile();

            return File(fileContent.fileContent, "application/octet-stream", fileContent.fileName);
        }
    }
}
