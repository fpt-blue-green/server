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

            if (contentFiles.Count < 3 || contentFiles.Count > 10)
            {
                _loggerService.Error("Error: Số lượng ảnh không hợp lệ. Chỉ được upload từ 3 đến 10 ảnh.");
                throw new InvalidOperationException("Số lượng ảnh không hợp lệ. Bạn chỉ được upload từ 3 đến 10 ảnh.");
            }

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

            // Upload Content Images
            _loggerService.Information("Start to upload content images: ");
            if (contentFiles.Any())
            {
                int imageIndex = 1;
                foreach (var file in contentFiles)
                {
                    try
                    {
                        var fileSuffix = $"content{imageIndex}";
                        var contentFileName = "Content/content_" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var contentDownloadUrl = await UploadImageAsync(file, "Images", user, fileSuffix);
                        contentDownloadUrls.Add(contentDownloadUrl);

                        // Add each image to InfluencerImages
                        var influencerImage = new InfluencerImage
                        {
                            Id = new Guid(),
                            InfluencerId = influencer.Id,
                            Url = contentDownloadUrl,
                            CreatedAt = DateTime.UtcNow,
                            ModifiedAt = DateTime.UtcNow
                        };

                        await _influencerImagesRepository.Create(influencerImage);
                        imageIndex++; // Tăng số thứ tự của ảnh
                    }
                    catch (Exception ex)
                    {
                        // Log content upload exception
                        _loggerService.Error(ex, $"Error uploading content image: {file.FileName}");
                        contentDownloadUrls.Add($"Error uploading content {file.FileName}: {ex.Message}");
                    }
                }
            }
            else
            {
                _loggerService.Error("Error uploading content images");
                throw new Exception("No content files provided");
            }

            _loggerService.Information("End to upload content images: ");
            return contentDownloadUrls;
        }
    }
}
