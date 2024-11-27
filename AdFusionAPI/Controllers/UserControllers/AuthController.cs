using AdFusionAPI.APIConfig;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers.UserControllers
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

        [HttpPost("login")]
        public async Task<ActionResult<UserTokenDTO>> Login([FromBody] LoginDTO loginDTO)
        {
            var userAgent = Request.Headers["User-Agent"].ToString();
            var result = await _authenService.Login(loginDTO, userAgent);
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout([FromBody] RefreshTokenDTO tokenDTO)
        {
            var userAgent = Request.Headers["User-Agent"].ToString();
            await _authenService.Logout(userAgent, tokenDTO.RefreshToken);
            return Ok();
        }

        [HttpPost("refreshToken")]
        public async Task<ActionResult<TokenResponseDTO>> RefreshToken([FromBody] RefreshTokenDTO tokenDTO)
        {
            var userAgent = Request.Headers["User-Agent"].ToString();
            var result = await _authenService.RefreshToken(tokenDTO, userAgent);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody] RegisterDTO userDTO)
        {
            await _authenService.Register(userDTO);
            return Ok();
        }

        [HttpPost("registerwiththirdparty")]
        public async Task<ActionResult> RegisterWithThirdParty([FromBody] RegisterThirdPartyDTO userDTO)
        {
            await _authenService.RegisterWithThirdParty(userDTO);
            return Ok();
        }

        [AuthRequired]
        [HttpPut("changePass")]
        public async Task<ActionResult<string>> ChangePass([FromBody] ChangePassDTO userDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _authenService.ChangePassword(userDTO, user);
            return Ok(result);
        }

        [HttpPut("forgotPass")]
        public async Task<ActionResult<string>> ForgotPass([FromBody] ForgotPasswordDTO userDTO)
        {
            var result = await _authenService.ForgotPassword(userDTO);
            return Ok(result);
        }

        [HttpPost("verify")]
        public async Task<ActionResult<bool>> Verify([FromBody] VerifyDTO data)
        {
            var result = await _authenService.Verify(data);
            return Ok(result);
        }
    }
}
