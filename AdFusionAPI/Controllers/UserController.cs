using AdFusionAPI.APIConfig;
using AutoMapper;
using BusinessObjects.DTOs.UserDTOs;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPatch("avatar")]
        [AuthRequired]
        public async Task<IActionResult> UpdateAvatar(IFormFile file)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var avatar = await _userService.UploadImageAsync(file, "Avatar", user);
            return Ok(avatar);
        }

        [HttpPost("images")]
        [AuthRequired]
        public async Task<IActionResult> UploadImages(List<IFormFile> images)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var avatar = await _userService.UploadContentImages(images, user);
            return Ok(avatar);
        }
    }
}
