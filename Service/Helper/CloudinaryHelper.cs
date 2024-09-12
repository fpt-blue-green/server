using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Service.Helper
{
    public static class CloudinaryHelper
    {
        static readonly Cloudinary _cloudinary;
        static CloudinaryHelper()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var account = new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }

        public static async Task<string> UploadImageAsync(IFormFile file, string folder)
        {
            if (file == null)
            {
                throw new Exception("Invalid File");
            }

            // Upload ảnh
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = folder,
                PublicId = Guid.NewGuid().ToString(), // Sử dụng Guid cho PublicId
                Overwrite = true
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }

        public static string GetValueAfterLastSlash(string url)
        {
            int lastSlashIndex = url.LastIndexOf('/');
            if (lastSlashIndex != -1)
            {
                var value = url.Substring(lastSlashIndex + 1);
                int dotIndex = value.LastIndexOf('.');
                if (dotIndex != -1)
                {
                    value = value.Substring(0, dotIndex);
                }

                return value;
            }

            return url;
        }

        public static async Task RemoveImageAsync(DeletionParams parameters)
        {
            await _cloudinary.DestroyAsync(parameters);
        }
    }
}
