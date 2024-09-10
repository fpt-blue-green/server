using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories;
using Serilog;

namespace Service
{
    public class UserService : IUserService
    {
        private static readonly IUserRepository _userRepository = new UserRepository();
        private static readonly IInfluencerRepository _influencerRepository = new InfluencerRepository();
        private static readonly IInfluencerImageRepository _influencerImagesRepository = new InfluencerImageRepository();
        private static ILogger _loggerService = new LoggerService().GetDbLogger();
        private static ISecurityService _securityService = new SecurityService();
        private static ConfigManager _configManager = new ConfigManager();
        private readonly IMapper _mapper;
        private readonly ConfigManager _config;
        private readonly Cloudinary _cloudinary;

        public UserService(IMapper mapper, IConfiguration config)
        {
            _mapper = mapper;
            var account = new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder, UserDTO user)
        {
            _loggerService.Information("Start to upload image: ");

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
            _loggerService.Information("End to upload image");

            return uploadResult.SecureUrl.ToString();
        }

        public async Task<List<string>> UploadContentImages(List<string> imageIds, List<IFormFile> contentFiles, UserDTO user)
        {
            var contentDownloadUrls = new List<string>();

            var influencer = await _influencerRepository.GetByUserId(user.Id);
            if (influencer == null)
            {
                throw new InvalidOperationException("Influencer không tồn tại");
            }

            // Lấy danh sách các ảnh hiện có của influencer từ DB bằng imageIds
            var influencerImages = await _influencerImagesRepository.GetByIds(imageIds);
            var remainingImageUrls = influencerImages.Select(img => img.Url).ToList();

            // Lấy danh sách các ảnh hiện có của influencer từ DB
            var existingImages = await _influencerImagesRepository.GetByInfluencerId(influencer.Id);
            var existingImageUrls = existingImages.Select(img => img.Url).ToList();

            // Tìm các URL ảnh trùng trong danh sách mới và danh sách hiện có
            var matchingImageUrls = remainingImageUrls.Intersect(existingImageUrls).ToList();

            // Kiểm tra nếu số ảnh không trùng và số ảnh mới nhỏ hơn 3
            if (matchingImageUrls.Count + contentFiles.Count < 3)
            {
                throw new InvalidOperationException("Influencer phải có ít nhất 3 ảnh.");
            }

            // Kiểm tra tổng số ảnh sau khi thêm mới không được vượt quá 10
            if (matchingImageUrls.Count + contentFiles.Count > 10)
            {
                throw new InvalidOperationException("Influencer chỉ được có tối đa 10 ảnh.");
            }

            // Nếu điều kiện hợp lệ, xóa các ảnh cũ không nằm trong danh sách mới
            foreach (var existingImage in existingImages)
            {
                var imagePath = GetValueAfterLastSlash(existingImage.Url);
                var link = $"Images/{imagePath}";
                if (!remainingImageUrls.Contains(existingImage.Url))
                {
                    var deletionParams = new DeletionParams(link);
                    await _cloudinary.DestroyAsync(deletionParams);

                    // Xóa ảnh từ DB
                    await _influencerImagesRepository.DeleteByUrl(existingImage.Url);
                }
            }

            // Thêm các ảnh mới từ contentFiles vào Cloudinary và DB
            _loggerService.Information("Start to upload content images: ");
            foreach (var file in contentFiles)
            {
                try
                {
                    var contentDownloadUrl = await UploadImageAsync(file, "Images", user);
                    contentDownloadUrls.Add(contentDownloadUrl);

                    var influencerImage = new InfluencerImage
                    {
                        Id = Guid.NewGuid(),
                        InfluencerId = influencer.Id,
                        Url = contentDownloadUrl,
                        CreatedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow
                    };

                    await _influencerImagesRepository.Create(influencerImage);

                }
                catch (Exception ex)
                {
                    _loggerService.Error(ex, $"Error uploading content image: {file.FileName}");
                    contentDownloadUrls.Add($"Error uploading content {file.FileName}: {ex.Message}");
                }
            }

            _loggerService.Information("End to upload content images: ");
            return contentDownloadUrls;
        }

        public string GetValueAfterLastSlash(string url)
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


    }
}
