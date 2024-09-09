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

        public async Task<List<string>> UploadContentImages(List<string> imageList, List<IFormFile> contentFiles, UserDTO user)
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

            // Lấy danh sách các ảnh hiện có của influencer từ DB
            var existingImages = await _influencerImagesRepository.GetByInfluencerId(influencer.Id);
            var existingImageCount = existingImages.Count();


            // Xóa các ảnh trong DB và trên Cloudinary nếu chúng không có trong danh sách mới
            foreach (var existingImage in existingImages)
            {
                var imagePath = GetValueAfterLastSlash(existingImage.Url);
                var link = $"Images/{imagePath}";
                if (!imageList.Contains(existingImage.Url))
                {
                    if (string.IsNullOrEmpty(imagePath))
                    {
                        throw new InvalidOperationException("Image Id không hợp lệ.");
                    }

                    var deletionParams = new DeletionParams(link);
                    await _cloudinary.DestroyAsync(deletionParams);

                    // Xóa ảnh từ DB
                    await _influencerImagesRepository.DeleteByUrl(existingImage.Url);
                }
            }

            // Đếm lại số ảnh sau khi xóa
            existingImageCount = await _influencerImagesRepository.GetCountByInfluencerId(influencer.Id);

            // Kiểm tra nếu số lượng ảnh còn lại và ảnh mới ít hơn 3
            if (existingImageCount < 3 && contentFiles.Count < (3 - existingImageCount))
            {
                throw new InvalidOperationException("Influencer phải có ít nhất 3 ảnh.");
            }

            // Kiểm tra tổng số ảnh sau khi thêm mới không được vượt quá 10
            if (existingImageCount + contentFiles.Count > 10)
            {
                throw new InvalidOperationException("Influencer chỉ được có tối đa 10 ảnh.");
            }

            // Thêm các ảnh mới từ contentFiles vào Cloudinary và DB
            _loggerService.Information("Start to upload content images: ");
            int imageIndex = existingImageCount + 1;

            foreach (var file in contentFiles)
            {
                try
                {
                    var fileSuffix = $"content{imageIndex}";
                    var contentFileName = $"{user.Id}_{fileSuffix}{Path.GetExtension(file.FileName)}";
                    var contentDownloadUrl = await UploadImageAsync(file, "Images", user, contentFileName);
                    contentDownloadUrls.Add(contentDownloadUrl);

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

        public string GetValueAfterLastSlash(string url)
        {
            // Tìm vị trí của dấu / cuối cùng trong URL
            int lastSlashIndex = url.LastIndexOf('/');

            // Nếu có dấu / trong URL, trích xuất phần sau dấu /
            if (lastSlashIndex != -1)
            {
                var value = url.Substring(lastSlashIndex + 1);

                // Loại bỏ phần mở rộng file (nếu có)
                int dotIndex = value.LastIndexOf('.');
                if (dotIndex != -1)
                {
                    value = value.Substring(0, dotIndex);
                }

                return value;
            }

            // Nếu không có dấu / trong URL, trả về giá trị gốc hoặc xử lý lỗi
            return url;
        }

    }
}
