using AdFusionAPI.APIConfig;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace AdFusionAPI.Controllers.AdminController
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
        [AdminRequired]
        public async Task<ActionResult<IEnumerable<AdminActionDTO>>> GetAdminAction()
        {
            var result = await _adminActionService.GetAdminAction();
            return Ok(result);
        }

        [HttpGet("export")]
        [AdminRequired]
        public async Task<IActionResult> DownloadDataFile()
        {
            var fileContent = await _adminActionService.GetDataFile();

            return File(fileContent.fileContent, "application/octet-stream", fileContent.fileName);
        }
    }
}
