using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Newtonsoft.Json;
using Repositories;
using Repositories.Implement;
using Repositories.Interface;
using Serilog;
using UAParser;
using static BusinessObjects.AuthEnumContainer;

namespace Service
{
    public class AuthService : IAuthService
    {
        private static IUserRepository _userRepository = new UserRepository();
        private static IUserDeviceRepository _userDeviceRepository = new UserDeviceRepository();
        private static ILogger _loggerService = new LoggerService().GetDbLogger();
        private static ISecurityService _securityService = new SecurityService();
        private static IEmailService _emailService = new EmailService();
        private static ConfigManager _configManager = new ConfigManager();
        private static EmailTemplate _emailTemplate = new EmailTemplate();
        private readonly IMapper _mapper;
        public AuthService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<UserTokenDTO> Login(LoginDTO loginDTO, string userAgent)
        {
            var user = new User();
            if(loginDTO.Password == null)
            {
                user = await _userRepository.GetUserByEmail(loginDTO.Email);
                if(user.Provider== (int)EAccountProvider.AdFusionAccount)
                {
                    throw new Exception();
                }
            }
            else
            {
                loginDTO.Password = _securityService.ComputeSha256Hash(loginDTO.Password);

                user = await _userRepository.GetUserByLoginDTO(loginDTO);
            }

            if (user == null)
            {
                throw new InvalidOperationException("Email hoặc mật khẩu không hợp lệ.");
            }


            if (user.IsBanned == true)
            {
                var bannedEntry = user.BannedUserUsers
                    .FirstOrDefault(b => b.UnbanDate != null && b.UnbanDate > DateTime.UtcNow && b.IsActive);

                if (bannedEntry != null)
                {
                    throw new InvalidOperationException("Người dùng đã bị cấm. Vui lòng liên hệ bộ phận hỗ trợ nếu có sự nhầm lẫn.");
                }
                else
                {
                    user.IsBanned = false;
                    await _userRepository.UpdateUser(user);
                }
            }
            _loggerService.Information($"Login: User with email {loginDTO.Email} login sucessfully.");
            return await GenerateUserTokens(user, userAgent);
        }

        protected async Task<UserTokenDTO> GenerateUserTokens(User user, string userAgent)
        {
            UserDTO userDTO = new UserDTO
            {
                Id = user.Id,
                Name = user.DisplayName,
                Email = user.Email,
                Role = (ERole)user.Role,
                Image = user.Avatar
            };

            var accessToken = await _securityService.GenerateAuthenToken(JsonConvert.SerializeObject(userDTO), 15);

            var userAgentConverted = GetBrowserInfo(userAgent);

            var refreshToken = string.Empty;
            var userDevice = user.UserDevices.FirstOrDefault(u => u.DeviceOperatingSystem == userAgentConverted.DeviceOperatingSystem
                                                            && u.BrowserName == userAgentConverted.BrowserName
                                                            && u.DeviceType == userAgentConverted.DeviceType);

            if (userDevice == null)
            {
                refreshToken = await _securityService.GenerateRefreshToken(JsonConvert.SerializeObject(userDTO));
                var newUserDevice = new UserDevice
                {
                    UserId = user.Id,
                    RefreshToken = refreshToken,
                    DeviceOperatingSystem = userAgentConverted.DeviceOperatingSystem,
                    BrowserName = userAgentConverted.BrowserName,
                    DeviceType = userAgentConverted.DeviceType,
                    RefreshTokenTime = DateTime.Now,
                };
                await _userDeviceRepository.Create(newUserDevice);
            }
            else
            {
                refreshToken = userDevice.RefreshToken;

                if (string.IsNullOrEmpty(refreshToken) || await _securityService.ValidateJwtAuthenToken(refreshToken!) == null)
                {
                    refreshToken = await _securityService.GenerateRefreshToken(JsonConvert.SerializeObject(userDTO));
                    userDevice.RefreshToken = refreshToken;
                    userDevice.RefreshTokenTime = DateTime.Now;
                }

                userDevice.LastLoginTime = DateTime.Now;
                await _userDeviceRepository.Update(userDevice);
            }

            UserTokenDTO userToken = new UserTokenDTO
            {
                Id = user.Id,
                Name = user.DisplayName,
                Email = user.Email,
                Role = (ERole)user.Role,
                Image = user.Avatar,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };

            return userToken;
        }

        protected BrowserInfo GetBrowserInfo(string userAgent)
        {
            if (userAgent == null)
            {
                throw new Exception("User-Agent is null");
            }

            var parser = Parser.GetDefault();
            var ua = parser.Parse(userAgent);

            // Xác định loại thiết bị
            var deviceType = ua.UA.Family.ToLower().Contains("mobile") ? "Mobile" : "Desktop";

            // Lấy thông tin hệ điều hành
            var operatingSystem = $"{ua.OS.Family}";

            return new BrowserInfo
            {
                BrowserName = ua.UA.Family.Replace("Mobile", "").Trim(), // Tên trình duyệt
                DeviceType = deviceType, // Loại thiết bị
                DeviceOperatingSystem = operatingSystem // Hệ điều hành
            };
        }

        public async Task RegisterWithThirdParty(RegisterThirdPartyDTO registerDTO)
        {
            if (registerDTO.AccountProvider == EAccountProvider.AdFusionAccount)
            {
                throw new Exception();
            }
            var user = await _userRepository.GetUserByEmail(registerDTO.Email);

            if(user != null)
            {
                throw new InvalidOperationException("Email đã tồn tại trong hệ thống. Vui lòng sử dụng tài khoản khác để đăng ký.");
            }
            var userNew = new User
            {
                Id = Guid.NewGuid(),
                Email = registerDTO!.Email,
                IsBanned = false,
                DisplayName = registerDTO.Name,
                Avatar = registerDTO.Image,
                IsDeleted = false,
                Provider = (int)registerDTO.AccountProvider,
                Role = (int)registerDTO.Role,
                CreatedAt = DateTime.Now,
            };
            await _userRepository.CreateUser(userNew);
        }

        public async Task<TokenResponseDTO> RefreshToken(RefreshTokenDTO tokenDTO, string userAgent)
        {
            var data = await _securityService.DecryptJWTAccessToken(tokenDTO.RefreshToken);

            if (data == null)
            {
                throw new UnauthorizedAccessException();
            }

            var userDTO = JsonConvert.DeserializeObject<UserDTO>(data);

            var userAgentConverted = GetBrowserInfo(userAgent);
            var userDevice = await _userDeviceRepository.GetUserDeviceByAgentAndUserID(userAgentConverted, userDTO!.Id);

            if (userDevice == null)
            {
                throw new KeyNotFoundException();
            }

            if (userDevice.RefreshTokenTime != null && userDevice.RefreshTokenTime!.Value.AddDays(30) < DateTime.Now)
            {
                throw new UnauthorizedAccessException();
            }

            var authenToken = await _securityService.GenerateAuthenToken(JsonConvert.SerializeObject(userDTO), 15);
            var refreshToken = await _securityService.GenerateRefreshToken(JsonConvert.SerializeObject(userDTO));

            userDevice.RefreshToken = refreshToken;
            userDevice.RefreshTokenTime = DateTime.Now;
            await _userDeviceRepository.Update(userDevice);

            var tokenResponse = new TokenResponseDTO
            {
                AccessToken = authenToken,
                RefreshToken = refreshToken
            };

            _loggerService.Information($"RefreshToken: User with email {userDTO?.Email} refresh token sucessfully.");
            return tokenResponse;
        }

        public async Task Logout(string userAgent, string refreshToken)
        {
            var userAgentConverted = GetBrowserInfo(userAgent);
            var userDevice = await _userDeviceRepository.GetUserDeviceByAgentAndToken(userAgentConverted, refreshToken);

            if (userDevice == null)
            {
                _loggerService.Warning($"Logout:  Refresh token Failed {refreshToken}.");
                return;
            }

            userDevice.RefreshToken = null;

            await _userDeviceRepository.Update(userDevice);
        }

        public async Task<string> Register(RegisterDTO registerDTO)
        {
            var userGet = await _userRepository.GetUserByEmail(registerDTO.Email);

            if (userGet != null)
            {
                throw new InvalidOperationException("Email đã tồn tại.");
            }

            var token = await _securityService.GenerateAuthenToken(JsonConvert.SerializeObject(registerDTO));
            var confirmationUrl = $"{_configManager.WebBaseUrl}/verify?action={(int)EAuthAction.Register}&token={token}";
            var body = _emailTemplate.authenTemplate.Replace("{projectName}", _configManager.ProjectName).Replace("{Action}", "Đăng ký tài khoản mới").Replace("{confirmLink}", confirmationUrl);
            // Gửi mail thông báo trong một tác vụ nền
            _ = Task.Run(async () => await _emailService.SendEmail(new List<string> { registerDTO.Email }, "Xác nhận đăng ký tài khoản mới", body));
            return "Đăng ký thành công. Vui lòng kiểm tra email để xác nhận.";
        }

        public async Task<string> ChangePassword(ChangePassDTO changePassDTO, UserDTO userData)
        {
            var loginDTO = new LoginDTO
            {
                Email = userData.Email!,
                Password = _securityService.ComputeSha256Hash(changePassDTO.OldPassword)
            };
            var userGet = await _userRepository.GetUserByLoginDTO(loginDTO);

            if (userGet == null)
            {
                throw new InvalidOperationException("Mật khẩu hiện tại không đúng.");
            }

            var newPasswordHash = _securityService.ComputeSha256Hash(changePassDTO.NewPassword);

            if (userGet.Password == newPasswordHash)
            {
                throw new InvalidOperationException("Mật khẩu mới không được trùng với mật khẩu cũ.");
            }

            userGet.Password = newPasswordHash;

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var tokenChangePass = await _securityService.GenerateAuthenToken(JsonConvert.SerializeObject(userGet, settings));

            var confirmationUrl = $"{_configManager.WebBaseUrl}/verify?action={(int)EAuthAction.ChangePass}&token={tokenChangePass}";

            var body = _emailTemplate.authenTemplate.Replace("{projectName}", _configManager.ProjectName).Replace("{Action}", "Thay đổi mật khẩu").Replace("{confirmLink}", confirmationUrl);

            _ = Task.Run(async () => await _emailService.SendEmail(new List<string> { userGet.Email }, "Xác nhận thay đổi mật khẩu", body));

            return "Thay đổi thành công. Vui lòng kiểm tra email để xác nhận.";
        }

        public async Task<string> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO)
        {
            var userGet = await _userRepository.GetUserByEmail(forgotPasswordDTO.Email);

            if (userGet == null)
            {
                throw new InvalidOperationException("Email chưa được đăng ký ở hệ thống.");
            }

            var newPasswordHash = _securityService.ComputeSha256Hash(forgotPasswordDTO.NewPassword);

            userGet.Password = newPasswordHash;

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var token = await _securityService.GenerateAuthenToken(JsonConvert.SerializeObject(userGet, settings));


            var confirmationUrl = $"{_configManager.WebBaseUrl}/verify?action={(int)EAuthAction.ForgotPassword}&token={token}";

            var body = _emailTemplate.authenTemplate.Replace("{projectName}", _configManager.ProjectName).Replace("{Action}", "Quên mật khẩu").Replace("{confirmLink}", confirmationUrl);

            _ = Task.Run(async () => await _emailService.SendEmail(new List<string> { forgotPasswordDTO.Email }, "Xác nhận quên mật khẩu", body));

            return "Yêu cầu cập nhập lại mật khẩu thành công. Vui lòng kiểm tra email để xác nhận.";
        }

        public async Task<bool> Verify(VerifyDTO data)
        {
            switch (data.Action)
            {
                case EAuthAction.Register:
                    await ValidateRegister(data.Token);
                    break;
                case EAuthAction.ChangePass:
                    await ValidateChangePass(data.Token);
                    break;
                case EAuthAction.ForgotPassword:
                    await ValidateForgotPass(data.Token);
                    break;
                default:
                    throw new Exception("Unvalid Validate Authen action!!");
            }
            return true;
        }

        public async Task ValidateRegister(string token)
        {
            var tokenDecrypt = await _securityService.ValidateJwtEmailToken(token);

            var registerDTO = JsonConvert.DeserializeObject<RegisterDTO>(tokenDecrypt);
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = registerDTO!.Email,
                Password = _securityService.ComputeSha256Hash(registerDTO.Password),
                IsBanned = false,
                DisplayName = registerDTO.DisplayName,
                IsDeleted = false,
                Provider = (int)EAccountProvider.AdFusionAccount,
                Role = (int)registerDTO.Role,
                CreatedAt = DateTime.Now,
            };

            await _userRepository.CreateUser(user);
        }

        public async Task ValidateChangePass(string token)
        {
            var tokenDecrypt = await _securityService.ValidateJwtEmailToken(token);
            var user = JsonConvert.DeserializeObject<User>(tokenDecrypt);
            await _userRepository.UpdateUser(user!);
        }

        public async Task ValidateForgotPass(string token)
        {
            var tokenDecrypt = await _securityService.ValidateJwtEmailToken(token);
            var user = JsonConvert.DeserializeObject<User>(tokenDecrypt);
            await _userRepository.UpdateUser(user!);
        }
    }
}
