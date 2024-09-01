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
            try
            {
                var user = await _userRepository.GetUserByRefreshToken(token);

                if (user == null)
                {
                    _loggerService.Warning($"Logout:  Refresh token Failed {token}.");
                }

                user.RefreshToken = null;

                await _userRepository.UpdateUser(user);
            }
            catch (Exception ex)
            {
                _loggerService.Error("Logout: " + ex.ToString());
            }
        }

        public async Task<ApiResponse<string>> Register(RegisterDTO registerDTO)
        {
            try
            {
                var userGet = await _userRepository.GetUserByEmail(registerDTO.Email);

                if (userGet != null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = EHttpStatusCode.Conflict,
                        Message = "Email đã tồn tại.",
                        Data = null
                    };
                }

                var token = await _securityService.GenerateAuthenToken(JsonConvert.SerializeObject(registerDTO));

                var confirmationUrl = $"{_configManager.WebBaseUrl}/verify?action={(int)EAuthAction.Register}&token={token}";

                var body = _emailTempalte.authenTemplate.Replace("{projectName}", _configManager.ProjectName).Replace("{Action}", "Đăng ký tài khoản mới").Replace("{confirmLink}", confirmationUrl);

                await _emailService.SendEmail(new List<string> { registerDTO.Email }, "Xác nhận đăng ký tài khoản mới", body);

                return new ApiResponse<string>
                {
                    StatusCode = EHttpStatusCode.Created,
                    Message = "Đăng ký thành công. Vui lòng kiểm tra email để xác nhận.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _loggerService.Error("Register: " + ex.ToString());
                return new ApiResponse<string>
                {
                    StatusCode = EHttpStatusCode.InternalServerError,
                    Message = _configManager.SeverErrorMessage,
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<string>> ChangePassword(ChangePassDTO changePassDTO, string token)
        {
            try
            {
                var userData = await _securityService.ValidateJwtToken(token);

                if (userData == null)
                {
                    throw new Exception($"Token Unvalid: {token}.");
                }

                var loginDTO = new LoginDTO
                {
                    Email = JsonConvert.DeserializeObject<UserDTO>(userData)!.Email!,
                    Password = _securityService.ComputeSha256Hash(changePassDTO.OldPassword)
                };
                var userGet = await _userRepository.GetUserByLoginDTO(loginDTO);

                if (userGet == null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = EHttpStatusCode.NotFound,
                        Message = "Mật khẩu hiện tại không trùng khớp.",
                        Data = null
                    };
                }

                var newPasswordHash = _securityService.ComputeSha256Hash(changePassDTO.NewPassword);


                if (userGet.Password == newPasswordHash)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = EHttpStatusCode.BadRequest,
                        Message = "Mật khẩu mới không được trùng với mật khẩu cũ.",
                        Data = null
                    };
                }

                userGet.Password = newPasswordHash;

                var tokenChangePass = await _securityService.GenerateAuthenToken(JsonConvert.SerializeObject(userGet));

                var confirmationUrl = $"{_configManager.WebBaseUrl}/verify?action={(int)EAuthAction.ChangePass}&token={tokenChangePass}";

                var body = _emailTempalte.authenTemplate.Replace("{projectName}", _configManager.ProjectName).Replace("{Action}", "Thay đổi mật khẩu").Replace("{confirmLink}", confirmationUrl);

                await _emailService.SendEmail(new List<string> { userGet.Email }, "Xác nhận thay đổi mật khẩu", body);

                return new ApiResponse<string>
                {
                    StatusCode = EHttpStatusCode.OK,
                    Message = "Thay đổi thành công. Vui lòng kiểm tra email để xác nhận.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _loggerService.Error("ChangePassword: " + ex.ToString());

                return new ApiResponse<string>
                {
                    StatusCode = EHttpStatusCode.InternalServerError,
                    Message = _configManager.SeverErrorMessage,
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<string>> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO)
        {
            try
            {
                var userGet = await _userRepository.GetUserByEmail(forgotPasswordDTO.Email);

                if (userGet == null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = EHttpStatusCode.BadRequest,
                        Message = "Email chưa được đăng ký ở hệ thống.",
                        Data = null
                    };
                }
                var newPasswordHash = _securityService.ComputeSha256Hash(forgotPasswordDTO.NewPassword);

                userGet.Password = newPasswordHash;

                var token = await _securityService.GenerateAuthenToken(JsonConvert.SerializeObject(userGet));

                var confirmationUrl = $"{_configManager.WebBaseUrl}/verify?action={(int)EAuthAction.ForgotPassword}&token={token}";

                var body = _emailTempalte.authenTemplate.Replace("{projectName}", _configManager.ProjectName).Replace("{Action}", "Quên mật khẩu").Replace("{confirmLink}", confirmationUrl);

                await _emailService.SendEmail(new List<string> { forgotPasswordDTO.Email }, "Xác nhận quên mật khẩu", body);

                return new ApiResponse<string>
                {
                    StatusCode = EHttpStatusCode.OK,
                    Message = "Yêu cầu cập nhập lại mật khẩu thành công. Vui lòng kiểm tra email để xác nhận.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _loggerService.Error("ForgotPassword: " + ex.ToString());

                return new ApiResponse<string>
                {
                    StatusCode = EHttpStatusCode.InternalServerError,
                    Message = _configManager.SeverErrorMessage,
                    Data = null
                };
            }
        }

        public async Task<bool> Verify(VerifyDTO data)
        {
            try
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
            catch (Exception ex)
            {
                _loggerService.Error("Verify: " + ex.ToString());
                return false;
            }
        }

        public async Task ValidateRegister(string token)
        {
            try
            {
                var tokenDecrypt = await _securityService.ValidateJwtToken(token);

                if (tokenDecrypt == null)
                {
                    throw new Exception($"Token Unvalid: {token}.");
                }

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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task ValidateChangePass(string token)
        {
            try
            {
                var tokenDecrypt = await _securityService.ValidateJwtToken(token);
                if (tokenDecrypt == null)
                {
                    throw new Exception($"Token Unvalid: {token}.");
                }

                var user = JsonConvert.DeserializeObject<User>(tokenDecrypt);
                await _userRepository.UpdateUser(user!);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task ValidateForgotPass(string token)
        {
            try
            {
                var tokenDecrypt = await _securityService.ValidateJwtToken(token);
                if (tokenDecrypt == null)
                {
                    throw new Exception($"Token Unvalid: {token}.");
                }

                var user = JsonConvert.DeserializeObject<User>(tokenDecrypt);
                await _userRepository.UpdateUser(user!);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
