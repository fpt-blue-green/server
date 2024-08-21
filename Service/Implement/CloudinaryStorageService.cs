using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Service.Interface;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Service.Implement
{
    public class CloudinaryStorageService : ICloudinaryStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryStorageService(IConfiguration config)
        {
            // Initialize Cloudinary client using the configuration
            var account = new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folderName)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = folderName, // E.g., "AdFusionImage"
                PublicId = Path.GetFileNameWithoutExtension(file.FileName),
                Overwrite = true
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }

        public async Task<Dictionary<string, List<string>>> UploadListImageAndAvatar(IFormFile avatarFile, List<IFormFile> contentFiles)
        {
            var downloadUrls = new Dictionary<string, List<string>>
            {
                { "Avatar", new List<string>() },
                { "Content", new List<string>() }
            };

            // Upload Avatar
            if (avatarFile != null)
            {
                var avatarFileName = "Avatar/avatar_" + Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);
                var avatarDownloadUrl = await UploadImageAsync(avatarFile, "Avatar");
                downloadUrls["Avatar"].Add(avatarDownloadUrl);
            }

            // Upload Content Images
            foreach (var file in contentFiles)
            {
                var contentFileName = "Content/content_" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var contentDownloadUrl = await UploadImageAsync(file, "Content");
                downloadUrls["Content"].Add(contentDownloadUrl);
            }

            return downloadUrls;
        }

        public async Task<bool> DeleteFileAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);
            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

            return deletionResult.Result == "ok";
        }
    }
}
