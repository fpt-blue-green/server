using AdFusionAPI.APIConfig;
using AutoMapper;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Helper;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly Utils _utils;

        public UserController(IUserService userService, IMapper mapper, Utils utils)
        {
            _userService = userService;
            _mapper = mapper;
            _utils = utils;
        }

        [HttpPatch("avatar")]
        [AuthRequired]
        public async Task<ActionResult<string>> UpdateAvatar(IFormFile file)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var avatar = await _utils.UploadAvatarAsync(file, "Avatar", user);
            return Ok(avatar);
        }
    }
}
