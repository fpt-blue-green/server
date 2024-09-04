using AutoMapper;
using BusinessObjects.DTOs;
using BusinessObjects.DTOs.AuthDTO;
using BusinessObjects.DTOs.AuthDTOs;
using BusinessObjects.DTOs.UserDTOs;
using BusinessObjects.Enum;
using BusinessObjects.Models;
using Newtonsoft.Json;
using Repositories.Implement;
using Repositories.Interface;
using Serilog;
using Service.Domain;
using Service.Implement.SystemService;
using Service.Interface;
using Service.Interface.HelperService;
using Service.Interface.UtilityServices;
using Service.Resources;
using static BusinessObjects.Enum.AuthEnumContainer;

namespace Service.Implement
{
    public class AuthService : IAuthService
    {
        private static IUserRepository _userRepository = new UserRepository();
        private static ILogger _loggerService = new LoggerService().GetDbLogger();
        private static ISecurityService _securityService = new SecurityService();
        private static IEmailService _emailService = new EmailService();
        private static ConfigManager _configManager = new ConfigManager();
        private static EmailTemplate _emailTempalte = new EmailTemplate();
        private readonly IMapper _mapper;
        public AuthService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<UserTokenDTO> Login(LoginDTO loginDTO)
        {
            loginDTO.Password = _securityService.ComputeSha256Hash(loginDTO.Password);

            var user = await _userRepository.GetUserByLoginDTO(loginDTO);

            if (user == null)
            {
                throw new InvalidOperationException("Email hoặc mật khẩu không hợp lệ.");
            }

            if (user.IsBanned == true)
            {
                var bannedEntry = user.BannedUserUsers
                    .FirstOrDefault(b => b.UnbanDate == null || b.UnbanDate > DateTime.UtcNow);

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
            UserDTO userDTO = new UserDTO
            {
                Id = user.Id,
                Name = user.DisplayName,
                Email = user.Email,
                Role = (ERole)user.Role,
                Image = user.Avatar
            };

            var accessToken = await _securityService.GenerateAuthenToken(JsonConvert.SerializeObject(userDTO));
            var refreshToken = await _securityService.GenerateRefreshToken(JsonConvert.SerializeObject(userDTO));

            UserTokenDTO userToken = new UserTokenDTO
            {
                Id = user.Id,
                Name = user.DisplayName,
                Email = user.Email,
                Role = (ERole)user.Role,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };

            user.RefreshToken = refreshToken;
            await _userRepository.UpdateUser(user);

            _loggerService.Information($"Login: User with email {loginDTO.Email} login sucessfully.");
            return userToken;
        }

        public async Task<TokenResponse> RefreshToken(RefreshTokenDTO tokenDTO)
        {
            var data = await _securityService.ValidateJwtToken(tokenDTO.Token);

            if (data == null)
            {
                throw new UnauthorizedAccessException();
            }

            var userDTO = JsonConvert.DeserializeObject<UserDTO>(data);

            var user = await _userRepository.GetUserByRefreshToken(tokenDTO.Token!);

            if (user == null)
            {
                throw new KeyNotFoundException();
            }

            var authenToken = await _securityService.GenerateAuthenToken(JsonConvert.SerializeObject(userDTO));
            var refreshToken = await _securityService.GenerateRefreshToken(JsonConvert.SerializeObject(userDTO));

            user.RefreshToken = refreshToken;
            await _userRepository.UpdateUser(user);

            var tokenResponse = new TokenResponse
            {
                AccessToken = authenToken,
                RefreshToken = refreshToken
            };

            _loggerService.Information($"RefreshToken: User with email {user.Email} refresh token sucessfully.");
            return tokenResponse;
        }

        public async Task Logout(string token)
        {
            var user = await _userRepository.GetUserByRefreshToken(token);

            if (user == null)
            {
                _loggerService.Warning($"Logout:  Refresh token Failed {token}.");
                return;
            }

            user.RefreshToken = null;

            await _userRepository.UpdateUser(user);
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
            var body = _emailTempalte.authenTemplate.Replace("{projectName}", _configManager.ProjectName).Replace("{Action}", "Đăng ký tài khoản mới").Replace("{confirmLink}", confirmationUrl);
            await _emailService.SendEmail(new List<string> { registerDTO.Email }, "Xác nhận đăng ký tài khoản mới", body);
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

            var tokenChangePass = await _securityService.GenerateAuthenToken(JsonConvert.SerializeObject(userGet));

            var confirmationUrl = $"{_configManager.WebBaseUrl}/verify?action={(int)EAuthAction.ChangePass}&token={tokenChangePass}";

            var body = _emailTempalte.authenTemplate.Replace("{projectName}", _configManager.ProjectName).Replace("{Action}", "Thay đổi mật khẩu").Replace("{confirmLink}", confirmationUrl);

            await _emailService.SendEmail(new List<string> { userGet.Email }, "Xác nhận thay đổi mật khẩu", body);

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

            var token = await _securityService.GenerateAuthenToken(JsonConvert.SerializeObject(userGet));

            var confirmationUrl = $"{_configManager.WebBaseUrl}/verify?action={(int)EAuthAction.ForgotPassword}&token={token}";

            var body = _emailTempalte.authenTemplate.Replace("{projectName}", _configManager.ProjectName).Replace("{Action}", "Quên mật khẩu").Replace("{confirmLink}", confirmationUrl);

            await _emailService.SendEmail(new List<string> { forgotPasswordDTO.Email }, "Xác nhận quên mật khẩu", body);

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
            var tokenDecrypt = await _securityService.ValidateJwtToken(token);

            var registerDTO = JsonConvert.DeserializeObject<RegisterDTO>(tokenDecrypt);
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = registerDTO!.Email,
                Password = _securityService.ComputeSha256Hash(registerDTO.Password),
                IsBanned = false,
                DisplayName = registerDTO.DisplayName,
                IsDeleted = false,
                Role = (int)registerDTO.Role,
                CreatedAt = DateTime.UtcNow,
            };

            await _userRepository.CreateUser(user);
        }

        public async Task ValidateChangePass(string token)
        {
            var tokenDecrypt = await _securityService.ValidateJwtToken(token);
            var user = JsonConvert.DeserializeObject<User>(tokenDecrypt);
            await _userRepository.UpdateUser(user!);
        }

        public async Task ValidateForgotPass(string token)
        {
            var tokenDecrypt = await _securityService.ValidateJwtToken(token);
            var user = JsonConvert.DeserializeObject<User>(tokenDecrypt);
            await _userRepository.UpdateUser(user!);
        }
    }
}
