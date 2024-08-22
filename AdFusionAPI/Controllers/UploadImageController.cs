using Microsoft.AspNetCore.Mvc;
using Service.Implement;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly CloudinaryStorageService _storageService;

        public UploadController(CloudinaryStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpPost()]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var fileUrl = await _storageService.UploadImageAsync(file, "AdFusionImage");
            return Ok(fileUrl);
        }
    }

}
