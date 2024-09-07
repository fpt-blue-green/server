using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service;

namespace AdFusionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly ICloudinaryStorageService _storageService;

        public UploadController(ICloudinaryStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpPost("image")]
        public async Task<ActionResult<string>> UploadImage(IFormFile file)
        {
            var fileUrl = await _storageService.UploadImageAsync(file, "AdFusionImage");

            return fileUrl.IsNullOrEmpty() ? BadRequest() : Ok(fileUrl);
        }

        [HttpPost("images")]
        public async Task<ActionResult<Dictionary<string, List<string>>>> UploadListImage(IFormFile avatar,  List<IFormFile> images)
        {
            var fileUrl = await _storageService.UploadListImageAndAvatar(avatar, images);

            return fileUrl.IsNullOrEmpty() ? BadRequest() : Ok(fileUrl);
        }

        [HttpDelete("image")]

        public async Task<IActionResult> DeleteFile(string publicId)
        {
            bool isDeleted = await _storageService.DeleteFileAsync(publicId);

            return isDeleted ? Ok(new { message = "File deleted successfully." }) : StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete the file.");
        }
    }

}
