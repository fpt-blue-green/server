﻿using AutoMapper;
using BusinessObjects.Enum;
using BusinessObjects.Models;
using BusinessObjects.ModelsDTO;
using BusinessObjects.ModelsDTO.AuthDTO;
using BusinessObjects.ModelsDTO.UserDTOs;
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
using static BusinessObjects.Enum.AuthenEnumContainer;

namespace Service.Implement
{
    public class AuthService : IAuthService
    {
        private static IUserRepository _userRepository = new UserRepository();
        private static ILogger _loggerService = new LoggerService().GetLogger();
        private static ISecurityService _securityService = new SecurityService();
        private static IEmailService _emailService = new EmailService();
        private static ConfigManager _configManager = new ConfigManager();
        private static EmailTemplate _emailTempalte = new EmailTemplate();
        private readonly IMapper _mapper;
        public AuthService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<ApiResponse<string>> Login(LoginDTO loginDTO)
        {
            try
            {
                loginDTO.Password = _securityService.ComputeSha256Hash(loginDTO.Password);

                var user = await _userRepository.GetUserByLoginDTO(loginDTO);

                if (user == null)
                {
                    _loggerService.Warning($"Login: User with email {loginDTO.Email} input wrong email/password.");
                    return new ApiResponse<string>
                    {
                        StatusCode = EHttpStatusCode.Unauthorized,
                        Message = "Email hoặc mật khẩu không hợp lệ.",
                        Data = null
                    };
                }

                if (user.IsBanned == true)
                {
                    var bannedEntry = user.BannedUserUsers
                        .FirstOrDefault(b => b.UnbanDate == null || b.UnbanDate > DateTime.UtcNow);

                    if (bannedEntry != null)
                    {
                        _loggerService.Warning($"Login: User with email {loginDTO.Email} has been banned.");
                        return new ApiResponse<string>
                        {
                            StatusCode = EHttpStatusCode.Forbidden,
                            Message = "Người dùng đã bị cấm. Vui lòng liên hệ bộ phận hỗ trợ nếu có sự nhầm lẫn.",
                            Data = null
                        };
                    }
                    else
                    {
                        user.IsBanned = false;
                        await _userRepository.UpdateUser(user);
                    }
                }

                await _userRepository.UpdateUser(user);

                UserTokenDTO userDTO = _mapper.Map<UserTokenDTO>(user);
                var token = await _securityService.GenerateJwtToken(JsonConvert.SerializeObject(userDTO), userDTO.Role == ERole.Admin);

                _loggerService.Information($"Login: User with email {loginDTO.Email} login sucessfully.");
                return new ApiResponse<string>
                {
                    StatusCode = EHttpStatusCode.OK,
                    Message = "Đăng nhập thành công.",
                    Data = token
                };
            }
            catch (Exception ex)
            {
                _loggerService.Error("Login: " + ex.ToString());
                return new ApiResponse<string>
                {
                    StatusCode = EHttpStatusCode.InternalServerError,
                    Message = _configManager.SeverErrorMessage,
                    Data = null
                };
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

                var token = await _securityService.GenerateJwtToken(JsonConvert.SerializeObject(registerDTO), false);

                var confirmationUrl = $"{_configManager.WebApiBaseUrl}/Auth/validateAuthen?action={(int)EAuthAction.Register}&token={token}";

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
                    return new ApiResponse<string>
                    {
                        StatusCode = EHttpStatusCode.BadRequest,
                        Message = "Lỗi thông tin người dùng. Vui lòng đăng nhập lại.",
                        Data = null
                    };
                }

                var email = JsonConvert.DeserializeObject<UserTokenDTO>(userData)!.Email;

                var userGet = await _userRepository.GetUserByEmail(email);

                if (userGet == null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = EHttpStatusCode.NotFound,
                        Message = "Người dùng không tồn tại.",
                        Data = null
                    };
                }

                var newPasswordHash = _securityService.ComputeSha256Hash(changePassDTO.NewPassword);

                if (userGet.Password == newPasswordHash)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = EHttpStatusCode.BadRequest,
                        Message = "Mật khẩu mới không được giống mật khẩu cũ.",
                        Data = null
                    };
                }

                userGet.Password = newPasswordHash;

                var tokenChangePass = await _securityService.GenerateJwtToken(JsonConvert.SerializeObject(userGet), false);

                var confirmationUrl = $"{_configManager.WebApiBaseUrl}/Auth/validateAuthen?action={(int)EAuthAction.ChangePass}&token={tokenChangePass}";

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
                        Message = "Email không chưa được đăng ký ở hệ thống.",
                        Data = null
                    };
                }
                var newPasswordHash = _securityService.ComputeSha256Hash(forgotPasswordDTO.NewPassword);

                userGet.Password = newPasswordHash;

                var token = await _securityService.GenerateJwtToken(JsonConvert.SerializeObject(userGet), false);

                var confirmationUrl = $"{_configManager.WebApiBaseUrl}/Auth/validateAuthen?action={(int)EAuthAction.ForgotPassword}&token={token}";

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

        public async Task<bool> ValidateAuthen(int action, string token)
        {
            try
            {
                switch ((EAuthAction)action)
                {
                    case EAuthAction.Register:
                        await ValidateRegister(token);
                        break;
                    case EAuthAction.ChangePass:
                        await ValidateChangePass(token);
                        break;
                    case EAuthAction.ForgotPassword:
                        await ValidateForgotPass(token);
                        break;
                    default:
                        throw new Exception("Unvalid Validate Authen action!!");
                }
                return true;
            }
            catch (Exception ex)
            {
                _loggerService.Error("ValidateAuthen: " + ex.ToString());
                return false;
            }
        }

        public async Task ValidateRegister(string token)
        {
            try
            {
                var tokenDescrypt = await _securityService.ValidateJwtToken(token);
                if (tokenDescrypt == null)
                {
                    throw new Exception("Invalid token.");
                }

                var registerDTO = JsonConvert.DeserializeObject<RegisterDTO>(tokenDescrypt);

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = registerDTO!.Email,
                    Password = _securityService.ComputeSha256Hash(registerDTO.Password),
                    IsBanned = false,
                    DisplayName = registerDTO.Displayname,
                    IsDeleted = false,
                    Role = (int)ERole.Influencer,
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
                var tokenDescrypt = await _securityService.ValidateJwtToken(token);
                if (tokenDescrypt == null)
                {
                    throw new Exception("Invalid token.");
                }

                var user = JsonConvert.DeserializeObject<User>(tokenDescrypt);
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
                var tokenDescrypt = await _securityService.ValidateJwtToken(token);
                if (tokenDescrypt == null)
                {
                    throw new Exception("Invalid token.");
                }

                var user = JsonConvert.DeserializeObject<User>(tokenDescrypt);
                await _userRepository.UpdateUser(user!);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
