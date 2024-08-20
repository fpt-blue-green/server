using BusinessObjects.DTOs.AuthDTOs;
using Microsoft.AspNetCore.Mvc;
using Service.Domain;
using Service.Interface;
using Service.Interface.UtilityServices;

namespace AdFusionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authenService;
        private readonly IEmailService _emailService;
        private readonly ConfigManager _config;

        public AuthController(IAuthService authenService, IEmailService emailService, ConfigManager config)
        {
            _authenService = authenService;
            _emailService = emailService;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await _authenService.Login(loginDTO);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO userDTO)
        {
            var result = await _authenService.Register(userDTO);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("verify")]
        public async Task<IActionResult> Verify([FromQuery] string token, [FromQuery] int action)
        {
            var result = await _authenService.Verify(action, token);
            return result == true ? Redirect("https://localhost:7244/swagger/index.html") : Redirect("https://localhost:7244/swagger/index.html");
        }
    }
}
