using AdFusionAPI.APIConfig;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers.AdminController
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
        public async Task<ActionResult<IEnumerable<BannedUserDTO>>> GetAdminAction()
        {
            var result = await _banService.GetBannedUsers();
            return Ok(result);
        }
    }
}
