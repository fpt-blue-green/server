using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service.Interface;

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
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var fileUrl = await _storageService.UploadImageAsync(file, "AdFusionImage");

            return fileUrl.IsNullOrEmpty() ? BadRequest() : Ok(fileUrl);
        }

        [HttpPost("images")]
        public async Task<IActionResult> UploadListImage(IFormFile avatar,  List<IFormFile> images)
        {
            var fileUrl = await _storageService.UploadListImageAndAvatar(avatar, images);

            return fileUrl.IsNullOrEmpty() ? BadRequest() : Ok(fileUrl);
        }

        [HttpDelete("image/{publicId}")]

        public async Task<IActionResult> DeleteFile(string publicId)
        {
            bool isDeleted = await _storageService.DeleteFileAsync(publicId);

            return isDeleted ? Ok(new { message = "File deleted successfully." }) : StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete the file.");
        }
    }

}
