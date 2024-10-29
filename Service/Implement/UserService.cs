using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
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

        public UserService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<User> GetUserById(Guid userId)
        {
            var user = await _userRepository.GetUserById(userId);
            return user;
        }

        public async Task<string> UploadAvatarAsync(IFormFile file, string folder, UserDTO user)
        {
            _loggerService.Information("Start to upload image.");

            if (file == null)
            {
                throw new InvalidOperationException("Tệp không hợp lệ.");
            }

            var userGet = await _userRepository.GetUserById(user.Id);
            if (userGet == null)
            {
                throw new InvalidOperationException("Người dùng không tồn tại");
            }

            // Upload ảnh
            var avatar = await CloudinaryHelper.UploadImageAsync(file, folder, user.Id);

            // Upload avatar to db
            userGet.Avatar = avatar.ToString();
            await _userRepository.UpdateUser(userGet);

            _loggerService.Information("End to upload image.");

            return avatar.ToString();
        }
    }
}
