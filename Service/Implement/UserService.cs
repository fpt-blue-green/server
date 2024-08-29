using AutoMapper;
using BusinessObjects.DTOs;
using BusinessObjects.DTOs.UserDTOs;
using BusinessObjects.Enum;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Repositories.Implement;
using Repositories.Interface;
using Serilog;
using Service.Domain;
using Service.Implement.SystemService;
using Service.Interface;
using Service.Interface.HelperService;

namespace Service.Implement
{
    public class UserService : IUserService
    {
        private static readonly IUserRepository _userRepository = new UserRepository();
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

        public async Task<ApiResponse<string>> UploadImageAsync(IFormFile file, string token)
        {
            try
            {
                _loggerService.Information("Start to upload avatar: ");

                if (file == null)
                {
                    throw new Exception("Invalid File");
                }
                //Kiểm tra token, nếu hợp thế thì mã hóa
                var tokenDecrypt = await _securityService.ValidateJwtToken(token);
                if (tokenDecrypt == null)
                {
                    throw new Exception($"Token Unvalid: {token}.");
                }
                var userDto = JsonConvert.DeserializeObject<UserDTO>(tokenDecrypt);

                //Lấy user để update
                var userGet = await _userRepository.GetUserById(userDto.Id);
                if (userGet == null)
                {
                    throw new Exception($"Unvalid User ID {userDto.Id}.");
                }

                //Upload ảnh
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    PublicId = Path.GetFileNameWithoutExtension(file.FileName),
                    Overwrite = true
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                userGet.Avatar = uploadResult.SecureUrl.ToString();
                _loggerService.Information("End to upload image");

                await _userRepository.UpdateUser(userGet);

                return new ApiResponse<string>
                {
                    StatusCode = EHttpStatusCode.OK,
                    Message = "Upload avatar thành công.",
                    Data = userGet.Avatar
                };
            }
            catch (Exception ex)
            {
                _loggerService.Error("Upload avatar: " + ex.ToString());
                return new ApiResponse<string>
                {
                    StatusCode = EHttpStatusCode.InternalServerError,
                    Message = _config.SeverErrorMessage,
                    Data = null
                };
            }
        }
    }
}
