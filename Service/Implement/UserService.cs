using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using Serilog;
using Service.Helper;
using System.Linq;
using System.Reflection;
using static BusinessObjects.AuthEnumContainer;

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

        public async Task DeleteUser(Guid userId)
        {
            var userGet = await _userRepository.GetUserById(userId);
            if (userGet == null)
            {
                throw new InvalidOperationException("Người dùng không tồn tại");
            }
            await _userRepository.DeleteUser(userId);
        }

        public async Task<FilterListResponse<UserDetailDTO>> GetAllUsers(UserFilterDTO filter)
        {
            try
            {
                var allUsers = (await _userRepository.GetUsers()); ;

                #region Filter

                if (filter.Roles != null && filter.Roles.Any())
                {
                    allUsers = allUsers.Where(i => filter.Roles.Contains((ERole)i.Role)).ToList();
                }
                if (filter.Providers != null && filter.Providers.Any())
                {
                    allUsers = allUsers.Where(i => filter.Providers.Contains((EAccountProvider)i.Provider)).ToList();
                }
               
                #endregion

                #region Search
                if (!string.IsNullOrEmpty(filter.Search))
                {
                    allUsers = allUsers.Where(i => i.DisplayName != null &&
                        i.DisplayName.Contains(filter.Search, StringComparison.OrdinalIgnoreCase)
                    );
                }
                #endregion

                #region Sort
                if (!string.IsNullOrEmpty(filter.SortBy))
                {
                    var propertyInfo = typeof(User).GetProperty(filter.SortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo != null)
                    {
                        allUsers = filter.IsAscending.HasValue && filter.IsAscending.Value
                            ? allUsers.OrderBy(i => propertyInfo.GetValue(i, null))
                            : allUsers.OrderByDescending(i => propertyInfo.GetValue(i, null));
                    }
                }
                #endregion

                #region Paging
                int pageSize = filter.PageSize;
                var pagedUsers = allUsers
                    .Skip((filter.PageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                #endregion

                return new FilterListResponse<UserDetailDTO>
                {
                    TotalCount = allUsers.Count(),
                    Items = _mapper.Map<List<UserDetailDTO>>(pagedUsers)
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Có lỗi xảy ra!", ex);
            }
        }

		public async Task<UserDetailDTO> GetUserById(Guid userId)
		{
			var user = await _userRepository.GetUserById(userId);
			return new UserDetailDTO
			{
				Id = user.Id,
				Email = user.Email,
				DisplayName = user?.DisplayName,
				Avatar = user?.Avatar,
				Role = user.Role,
				Wallet = user.Wallet,
				Provider = user.Provider,
				CreatedAt = user.CreatedAt
			};
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
