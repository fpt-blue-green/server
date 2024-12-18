﻿using AdFusionAPI.APIConfig;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers.UsersControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPatch("avatar")]
        [AuthRequired]
        public async Task<ActionResult<string>> UpdateAvatar(IFormFile file)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var avatar = await _userService.UploadAvatarAsync(file, "Avatar", user);
            return Ok(avatar);
        }

        [HttpGet]
        [AdminRequired]
        public async Task<ActionResult<FilterListResponse<UserDetailDTO>>> GetExploreInfluencer([FromQuery] UserFilterDTO filterDTO)
        {
            var result = await _userService.GetAllUsers(filterDTO);
            return Ok(result);
        }

        [HttpPost("delete")]
        [AuthRequired]
        public async Task<ActionResult<string>> DeleteUser(Guid userId)
        {
            await _userService.DeleteUser(userId);
            return Ok();
        }

        [HttpGet("loginHistory")]
        [AuthRequired]
        public async Task<ActionResult<IEnumerable<UserDeviceDTO>>> GetInfluencerLoginHistory()
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _userService.GetUserLoginHistory(user);
            return Ok(result);

        }

        [HttpGet("paymentHistory")]
        [AuthRequired]
        public async Task<ActionResult> GetUserPayments([FromQuery] FilterDTO filter)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _userService.GetUserPayments(user, filter);
            return Ok(result);
        }

        [HttpGet("wallet")]
        [AuthRequired]
        public async Task<ActionResult> GetUserWallet()
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _userService.GetUserWallet(user);
            return Ok(result);
        }
    }
}
