using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Repositories;
using Repositories.Implement;
using Repositories.Interface;
using Serilog;
using Service.Helper;
using System.Reflection;
using static BusinessObjects.AuthEnumContainer;

namespace Service
{
    public class UserService : IUserService
    {
        private static IUserDeviceRepository _userDeviceRepository = new UserDeviceRepository();
        private static readonly IUserRepository _userRepository = new UserRepository();
        private static readonly IPaymentRepository _paymentRepository = new PaymentRepository();
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

        public async Task<IEnumerable<UserDeviceDTO>> GetUserLoginHistory(UserDTO user)
        {
            var userDevices = await _userDeviceRepository.GetByUserId(user.Id);
            if (userDevices == null)
            {
                throw new KeyNotFoundException();
            }
            var result = _mapper.Map<IEnumerable<UserDeviceDTO>>(userDevices);
            return result;
        }

        public async Task<FilterListResponse<UserPaymentDTO>> GetUserPayments(UserDTO userDTO, FilterDTO filter)
        {
            var userPayments = await _userRepository.GetUserPayments(userDTO.Id);

            #region Sort
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                var propertyInfo = typeof(UserPaymentDTO).GetProperty(filter.SortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    userPayments = filter.IsAscending.HasValue && filter.IsAscending.Value
                        ? userPayments.OrderBy(i => propertyInfo.GetValue(i, null))
                        : userPayments.OrderByDescending(i => propertyInfo.GetValue(i, null));
                }
            }
            #endregion

            #region Paging
            int pageSize = filter.PageSize;
            int totalCount = userPayments.Count();
            userPayments = userPayments
                .Skip((filter.PageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            #endregion

            return new FilterListResponse<UserPaymentDTO>
            {
                TotalCount = totalCount,
                Items = userPayments
            };
        }

        public async Task<UserWalletDTO> GetUserWallet(UserDTO userDTO)
        {
            var currentAmount = (await _userRepository.GetUserById(userDTO.Id)).Wallet;
            var paymentWithDraw = await _paymentRepository.GetWithDrawPaymentHistoryByUserId(userDTO.Id);
            var spendAmount = paymentWithDraw.Sum(p => p.NetAmount) ?? 0;

            if (userDTO.Role == ERole.Brand)
            {
                var userPayments = await _userRepository.GetUserPayments(userDTO.Id);
                spendAmount += userPayments.Where(p => p.Type == EPaymentType.BrandPayment
                                                                || p.Type == EPaymentType.BuyPremium
                                                                || p.Type == EPaymentType.Deposit)
                                                           .Sum(p => p.Amount);
            }
            
            return new UserWalletDTO
            {
                CurrentAmount = currentAmount,
                SpendAmount = spendAmount
            };
        }
    }
}
