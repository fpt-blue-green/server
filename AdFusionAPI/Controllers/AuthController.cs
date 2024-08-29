using AdFusionAPI.APIConfig;
using BusinessObjects.DTOs.AuthDTO;
using BusinessObjects.DTOs.AuthDTOs;
using BusinessObjects.DTOs.UserDTOs;
using Microsoft.AspNetCore.Mvc;
using Service.Domain;
using Service.Interface;

namespace AdFusionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authenService;
        private readonly ConfigManager _config;

        public AuthController(IAuthService authenService, ConfigManager config)
        {
            _authenService = authenService;
            _config = config;
        }

        [ProducesResponseType(200, Type = typeof(UserTokenDTO))]
        [NoAuthRequired]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await _authenService.Login(loginDTO);
            return Ok(result);
        }

        [NoAuthRequired]
        [HttpPost("logout")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenDTO tokenDTO)
        {
            await _authenService.Logout(tokenDTO.Token);
            return Ok();
        }

        [NoAuthRequired]
        [HttpPost("refreshToken")]
        [ProducesResponseType(200, Type = typeof(TokenResponse))]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO tokenDTO)
        {
            var result = await _authenService.RefreshToken(tokenDTO);
            return Ok(result);
        }

        [NoAuthRequired]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO userDTO)
        {
            var result = await _authenService.Register(userDTO);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("changePass")]
        public async Task<IActionResult> ChangePass([FromBody] ChangePassDTO userDTO)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _authenService.ChangePassword(userDTO, token);
            return StatusCode((int)result.StatusCode, result);
        }

        [NoAuthRequired]
        [HttpPut("forgotPass")]
        public async Task<IActionResult> ForgotPass([FromBody] ForgotPasswordDTO userDTO)
        {
            var result = await _authenService.ForgotPassword(userDTO);
            return StatusCode((int)result.StatusCode, result);
        }

        [NoAuthRequired]
        [HttpGet("verify")]
        public async Task<IActionResult> Verify([FromQuery] string token, [FromQuery] int action)
        {
            var result = await _authenService.Verify(action, token);
            return result == true ? Redirect("https://localhost:7244/swagger/index.html") : Redirect("https://localhost:7244/swagger/index.html");
        }
    }
}
