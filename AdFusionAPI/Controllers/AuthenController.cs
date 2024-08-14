using BusinessObjects.ModelsDTO.AuthenDTO;
using Microsoft.AspNetCore.Mvc;
using Service.Domain;
using Service.Interface;

namespace AdFusionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenController : Controller
    {
        private readonly IAuthenService _authenService;
        private readonly IEmailService _emailService;
        private readonly ConfigManager _config;

        public AuthenController(IAuthenService authenService, IEmailService emailService, ConfigManager config)
        {
            _authenService = authenService;
            _emailService = emailService;
            this._config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await _authenService.Login(loginDTO);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
