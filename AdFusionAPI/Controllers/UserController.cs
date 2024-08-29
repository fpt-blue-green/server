using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

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

        [HttpPatch]
        public async Task<IActionResult> UpdateAvatar(IFormFile file)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var influencer = await _userService.UploadImageAsync(file, token);
            return StatusCode((int)influencer.StatusCode, influencer);
        }
    }
}
