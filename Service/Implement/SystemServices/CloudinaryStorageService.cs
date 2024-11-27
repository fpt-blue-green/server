using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Service
{
    public class CloudinaryStorageService : ICloudinaryStorageService
    {
        private readonly Cloudinary _cloudinary;
        private static ILogger _loggerService = new LoggerService().GetDbLogger();

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
            try
            {
                _loggerService.Information("Start to upload image: ");
                if (file == null)
                {
                    throw new Exception("Invalid File");
                }

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    Folder = folderName, // E.g., "AdFusionImage"
                    PublicId = Path.GetFileNameWithoutExtension(file.FileName),
                    Overwrite = true
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                _loggerService.Information("End to upload image");
                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                _loggerService.Error(ex.ToString());
                return string.Empty;
            }
        }


        public async Task<Dictionary<string, List<string>>> UploadListImageAndAvatar(IFormFile avatarFile, List<IFormFile> contentFiles)
        {
            var downloadUrls = new Dictionary<string, List<string>>
            {
                { "Avatar", new List<string>() },
                { "Content", new List<string>() }
            };

            try
            {
                // Upload Avatar
                _loggerService.Information("Start to upload list image: ");
                if (avatarFile != null && contentFiles.Any())
                {
                    try
                    {
                        var avatarFileName = "Avatar/avatar_" + Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);
                        var avatarDownloadUrl = await UploadImageAsync(avatarFile, "Avatar");
                        downloadUrls["Avatar"].Add(avatarDownloadUrl);
                    }
                    catch (Exception ex)
                    {
                        // Log avatar upload exception
                        _loggerService.Error(ex, "Error uploading avatar image");
                        downloadUrls["Avatar"].Add($"Error uploading avatar: {ex.Message}");
                    }
                }
                else
                {
                    throw new Exception("Invalid File");
                }

                // Upload Content Images
                foreach (var file in contentFiles)
                {
                    try
                    {
                        var contentFileName = "Content/content_" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var contentDownloadUrl = await UploadImageAsync(file, "Content");
                        downloadUrls["Content"].Add(contentDownloadUrl);
                    }
                    catch (Exception ex)
                    {
                        // Log content upload exception
                        _loggerService.Error(ex, $"Error uploading content image: {file.FileName}");
                        downloadUrls["Content"].Add($"Error uploading content {file.FileName}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _loggerService.Error(ex, "Error uploading images");
                throw; // Rethrow the exception if you want it to bubble up
            }
            _loggerService.Information("End to upload list image: ");
            return downloadUrls;
        }

        public async Task<bool> DeleteFileAsync(string publicId)
        {
            try
            {
                _loggerService.Information("Start to delete image: ");
                if (string.IsNullOrEmpty(publicId))
                {
                    throw new Exception("Invalid Image Id");
                }

                var deletionParams = new DeletionParams(publicId);
                var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

                _loggerService.Information("End to delete image: ");
                return deletionResult.Result == "ok";
            }
            catch (Exception ex)
            {
                // Log the exception
                _loggerService.Error(ex, $"Error deleting file with Public ID: {publicId}");

                // Optionally, handle the error or return false to indicate failure
                return false;
            }
        }
    }
}
