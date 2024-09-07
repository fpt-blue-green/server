using Microsoft.AspNetCore.Http;

namespace Service
{
    public interface ICloudinaryStorageService
    {
        Task<string> UploadImageAsync(IFormFile file, string folderName);
        Task<Dictionary<string, List<string>>> UploadListImageAndAvatar(IFormFile avatarFile, List<IFormFile> contentFiles);
        Task<bool> DeleteFileAsync(string publicId);
    }
}
