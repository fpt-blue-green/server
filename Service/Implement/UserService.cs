using AutoMapper;
using BusinessObjects.DTOs;
using BusinessObjects.DTOs.UserDTOs;
using BusinessObjects.Enum;
using BusinessObjects.Models;
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
        private static readonly IUserRepository _repository = new UserRepository();
        private static ILogger _loggerService = new LoggerService().GetDbLogger();
        private static ISecurityService _securityService = new SecurityService();
        private static ConfigManager _configManager = new ConfigManager();
        private readonly IMapper _mapper;
        private readonly ConfigManager _config;
        private readonly Cloudinary _cloudinary;

        public UserService(IMapper mapper, IConfiguration config)
        {
            _mapper = mapper;
            // Initialize Cloudinary client using the configuration
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

                var tokenDescrypt = await _securityService.ValidateJwtToken(token);
                if (tokenDescrypt == null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = EHttpStatusCode.BadRequest,
                        Message = _configManager.TokenInvalidErrorMessage,
                        Data = null
                    };
                }

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    PublicId = Path.GetFileNameWithoutExtension(file.FileName),
                    Overwrite = true
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                _loggerService.Information("End to upload image");

<<<<<<< HEAD
                var user = JsonConvert.DeserializeObject<UserTokenDTO>(tokenDescrypt);
=======
                var user = JsonConvert.DeserializeObject<UserDTO>(tokenDescrypt);
>>>>>>> 36665ee ([ADF-96] resolve conflict)

                user.Avatar = uploadResult.SecureUrl.ToString();
                var userUpdated = _mapper.Map<User>(user);

                await _repository.UpdateUser(userUpdated);

                return new ApiResponse<string>
                {
                    StatusCode = EHttpStatusCode.OK,
                    Message = "Upload avatar thành công.",
                    Data = user.Avatar
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
