using AutoMapper;
using BusinessObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories;
using Serilog;
using Service.Helper;

namespace Service
{
    public class UserService : IUserService
    {
        private static readonly IUserRepository _userRepository = new UserRepository();
        private static ILogger _loggerService = new LoggerService().GetDbLogger();
        private static ISecurityService _securityService = new SecurityService();
        private static ConfigManager _configManager = new ConfigManager();
        private readonly IMapper _mapper;
        private readonly ConfigManager _config;

        public UserService(IMapper mapper, IConfiguration config)
        {
            _mapper = mapper;
        }

        public async Task<string> UploadAvatarAsync(IFormFile file, string folder, UserDTO user)
        {
            _loggerService.Information("Start to upload image: ");

            if (file == null)
            {
                throw new Exception("Invalid File");
            }

            var userGet = await _userRepository.GetUserById(user.Id);
            if (userGet == null)
            {
                throw new InvalidOperationException("User không tồn tại");
            }

            // Upload ảnh
            var avatar = await CloudinaryHelper.UploadImageAsync(file, folder);

            // Upload avatar to db
            userGet.Avatar = avatar.ToString();
            await _userRepository.UpdateUser(userGet);

            _loggerService.Information("End to upload image");

            return avatar.ToString();
        }
    }
}
