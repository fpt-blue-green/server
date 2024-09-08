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

        public async Task<string> UploadImageAsync(IFormFile file, string folder, UserDTO user, string fileSuffix)
        {
            _loggerService.Information("Start to upload avatar: ");

            if (file == null)
            {
                throw new Exception("Invalid File");
            }

            //Lấy user để update
            var userGet = await _userRepository.GetUserById(user.Id);
            if (userGet == null)
            {
                throw new InvalidOperationException("User không tồn tại");
            }

            //Upload ảnh
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = folder,
                PublicId = $"{user.Id}_{fileSuffix}",
                Overwrite = true
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            userGet.Avatar = uploadResult.SecureUrl.ToString();
            _loggerService.Information("End to upload image");

            await _userRepository.UpdateUser(userGet);

            return userGet.Avatar;
        }

        public async Task<List<string>> UploadContentImages(List<IFormFile> contentFiles, UserDTO user)
        {
            var contentDownloadUrls = new List<string>();

            var userGet = await _userRepository.GetUserById(user.Id);
            if (userGet == null)
            {
                throw new InvalidOperationException("User không tồn tại");
            }

            var influencer = await _influencerRepository.GetByUserId(user.Id);
            if (influencer == null)
            {
                throw new InvalidOperationException("Influencer không tồn tại");
            }

            // Lấy danh sách các ảnh hiện có của influencer
            var existingImages = await _influencerImagesRepository.GetByInfluencerId(influencer.Id);
            var existingImagesOrdered = existingImages
                .OrderBy(img => img.CreatedAt)
                .ToList();

            int existingImageCount = existingImages.Count();

            if (existingImageCount < 3 && contentFiles.Count < (3 - existingImageCount))
            {
                throw new InvalidOperationException("Influencer must have at least 3 images.");
            }

            List<InfluencerImage> imagesToUpdate = new List<InfluencerImage>();
            if (existingImageCount + contentFiles.Count > 10)
            {
                int excessImages = (existingImageCount + contentFiles.Count) - 10;

                // Lấy các ảnh cũ nhất để thay thế
                imagesToUpdate = existingImagesOrdered
                    .Take(excessImages)
                    .ToList();
            }

            _loggerService.Information("Start to upload content images: ");
            int imageIndex = existingImageCount + 1;

            foreach (var file in contentFiles)
            {
                try
                {
                    if (imageIndex == 11)
                    {
                        imageIndex = 1;
                    }

                    var fileSuffix = $"content{imageIndex}";
                    var contentFileName = "Content/content_" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var contentDownloadUrl = await UploadImageAsync(file, "Images", user, fileSuffix);
                    contentDownloadUrls.Add(contentDownloadUrl);

                    // Thêm hoặc cập nhật ảnh
                    if (imagesToUpdate.Any())
                    {
                        // Cập nhật ảnh cũ với URL và thời gian mới
                        var imageToUpdate = imagesToUpdate.First();
                        imageToUpdate.Url = contentDownloadUrl;
                        imageToUpdate.ModifiedAt = DateTime.UtcNow;

                        await _influencerImagesRepository.Update(imageToUpdate);
                        imagesToUpdate.Remove(imageToUpdate);
                    }
                    else
                    {
                        // Tạo mới ảnh nếu chưa có ảnh nào cần thay thế
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

                    imageIndex++;
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
    }
}
