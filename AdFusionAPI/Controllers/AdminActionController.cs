using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminActionController : Controller
    {
        private readonly IAdminActionService _adminActionService;

        public AdminActionController(IAdminActionService adminActionService)
        {
            _adminActionService = adminActionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminActionDTO>>> GetAdminAction()
        {
            var result = await _adminActionService.GetAdminAction();
            return Ok(result);
        }

        [HttpGet("export")]
        public async Task<IActionResult> DownloadDataFile()
        {
            var fileContent = await _adminActionService.GetDataFile();

            return File(fileContent.fileContent, "application/octet-stream", fileContent.fileName);
        }
    }
}
