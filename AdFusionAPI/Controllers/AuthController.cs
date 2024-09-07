﻿using AdFusionAPI.APIConfig;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await _authenService.Login(loginDTO);
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenDTO tokenDTO)
        {
            await _authenService.Logout(tokenDTO.Token);
            return Ok();
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO tokenDTO)
        {
            var result = await _authenService.RefreshToken(tokenDTO);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO userDTO)
        {
            var result = await _authenService.Register(userDTO);
            return Ok(result);
        }

        [AuthRequired]
        [HttpPut("changePass")]
        public async Task<IActionResult> ChangePass([FromBody] ChangePassDTO userDTO)
        {
            var user = (UserDTO)HttpContext.Items["user"]!;
            var result = await _authenService.ChangePassword(userDTO, user);
            return Ok(result);
        }

        [HttpPut("forgotPass")]
        public async Task<IActionResult> ForgotPass([FromBody] ForgotPasswordDTO userDTO)
        {
            var result = await _authenService.ForgotPassword(userDTO);
            return Ok(result);
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyDTO data)
        {
            var result = await _authenService.Verify(data);
            return Ok(result);
        }
    }
}
