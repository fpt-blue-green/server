using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserService _userRepository, IMapper mapper)
        {
            _userRepository = _userRepository;
            _mapper = mapper;
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateAvatar(IFormFile file)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var influencer = await _userRepository.UploadImageAsync(file, token);
            return StatusCode((int)influencer.StatusCode, influencer);
        }
    }
}
