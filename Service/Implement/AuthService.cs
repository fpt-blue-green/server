using BusinessObjects.Enum;
using BusinessObjects.Models;
using BusinessObjects.DTOs;
using BusinessObjects.DTOs.AuthDTOs;
using BusinessObjects.DTOs.UserDTOs;
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
        private static IAuthRepository _authenRepository = new AuthRepository();
        private static ILogger _loggerService = new LoggerService().GetLogger();
        private static ISecurityService _securityService = new SecurityService();
        private static IEmailService _emailService = new EmailService();
        private static ConfigManager _configManager = new ConfigManager();
        private static EmailTemplate _emailTempalte = new EmailTemplate();


        public async Task<ApiResponse<string>> Login(LoginDTO loginDTO)
        {
            try
            {
                // Bước 1: Mã hóa mật khẩu của người dùng trước khi kiểm tra
                loginDTO.Password = _securityService.ComputeSha256Hash(loginDTO.Password);

                // Bước 2: Lấy thông tin người dùng từ repository dựa trên thông tin đăng nhập
                var user = await _authenRepository.GetUserByLoginDTO(loginDTO);

                // Bước 3: Kiểm tra nếu không tìm thấy người dùng hoặc thông tin đăng nhập không hợp lệ
                if (user == null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        Message = "Email hoặc mật khẩu không hợp lệ.",
                        Data = null
                    };
                }

                // Bước 4: Kiểm tra nếu người dùng bị cấm (banned)
                if (user.IsBanned == true)
                {
                    var bannedEntry = user.BannedUserUsers
                        .FirstOrDefault(b => b.UnbanDate == null || b.UnbanDate > DateTime.UtcNow);

                    // Bước 5: Nếu người dùng vẫn đang bị cấm, trả về lỗi
                    if (bannedEntry != null)
                    {
                        return new ApiResponse<string>
                        {
                            StatusCode = HttpStatusCode.Forbidden,
                            Message = "Người dùng đã bị cấm. Vui lòng liên hệ bộ phận hỗ trợ nếu có sự nhầm lẫn.",
                            Data = null
                        };
                    }
                    else
                    {
                        // Bước 6: Nếu không còn lý do cấm, hủy bỏ trạng thái cấm và cập nhật thông tin người dùng
                        user.IsBanned = false;
                        await _authenRepository.UpdateUser(user);
                    }
                }

                // Bước 7: Cập nhật ngày đăng nhập cuối cùng của người dùng
                await _authenRepository.UpdateUser(user);

                // Bước 8: Tạo DTO cho người dùng và sinh JWT token
                UserDTO userDTO = new UserDTO(user);
                var token = await _securityService.GenerateJwtToken(JsonConvert.SerializeObject(userDTO), userDTO.Role == (int)ERole.Admin);

                // Bước 9: Trả về phản hồi đăng nhập thành công kèm theo token
                return new ApiResponse<string>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Đăng nhập thành công.",
                    Data = token
                };
            }
            catch (Exception ex)
            {
                // Bước 10: Xử lý ngoại lệ và trả về lỗi máy chủ nội bộ
                _loggerService.Information(ex.ToString());
                return new ApiResponse<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = _configManager.SeverErrorMessage,
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<string>> Register(RegisterDTO registerDTO)
        {
            try
            {
                // 1. Kiểm tra xem email đã tồn tại trong hệ thống chưa.
                var userGet = await _authenRepository.GetUserByEmail(registerDTO.Email);

                // 2. Nếu email đã tồn tại, trả về phản hồi với mã lỗi.
                if (userGet != null)
                {
                    return new ApiResponse<string>
                    {
                        StatusCode = HttpStatusCode.Conflict,
                        Message = "Email đã tồn tại.",
                        Data = null
                    };
                }

                // 3. Tạo token JWT chứa thông tin đăng ký của người dùng với quyền User.
                var token = await _securityService.GenerateJwtToken(JsonConvert.SerializeObject(registerDTO), false);

                // 4. Tạo liên kết xác nhận email với token đã tạo.
                var confirmationUrl = $"{_configManager.WebApiBaseUrl}/Authen/validateAuthen?action={(int)EAuthAction.Register}&token={token}";

                var body = _emailTempalte.authenTemplate.Replace("{projectName}", _configManager.ProjectName).Replace("{Action}", "Đăng ký tài khoản mới").Replace("{confirmLink}", confirmationUrl);

                // 5. Gửi email xác nhận chứa liên kết đến người dùng.
                await _emailService.SendEmail(registerDTO.Email, "Xác nhận đăng ký tài khoản mới", body);

                // 6. Trả về phản hồi thành công với liên kết xác nhận.
                return new ApiResponse<string>
                {
                    StatusCode = HttpStatusCode.Created,
                    Message = "Đăng ký thành công. Vui lòng kiểm tra email để xác nhận.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                // 7. Nếu có lỗi xảy ra, trả về phản hồi lỗi kèm theo thông báo lỗi.
                return new ApiResponse<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = _configManager.SeverErrorMessage,
                    Data = null
                };
            }
        }

        public async Task<bool> Verify(int action, string token)
        {
            try
            {
                switch (action)
                {
                    case (int)EAuthAction.Register:
                        await ValidateRegister(token);
                        break;
                    case (int)EAuthAction.ChangePass:
                        break;
                    case (int)EAuthAction.ForgotPassword:
                        break;
                    default:
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
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
                    DisplayName = registerDTO.DisplayName,
                    IsDeleted = false,
                    Role = (int)ERole.Influencer,
                    CreatedAt = DateTime.UtcNow,
                };

                await _authenRepository.CreateUser(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
