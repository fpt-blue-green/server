using BusinessObjects.Enum;
using BusinessObjects.Models;
using BusinessObjects.ModelsDTO;
using BusinessObjects.ModelsDTO.AuthenDTO;
using Newtonsoft.Json;
using Repositories.Implement;
using Repositories.Interface;
using Service.Domain;
using Service.Interface;
using Service.Resources;
using static BusinessObjects.Enum.AuthenEnumContainer;

namespace Service.Implement
{
    public class AuthenService : IAuthenService
    {
        private static IAuthenRepository _authenRepository = new AuthenRepository();
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
                if (user.IsDeleted == true)
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
                        user.IsDeleted = false;
                        await _authenRepository.UpdateUser(user);
                    }
                }

                // Bước 7: Cập nhật ngày đăng nhập cuối cùng của người dùng
                await _authenRepository.UpdateUser(user);

                // Bước 8: Tạo DTO cho người dùng và sinh JWT token
                UserDTO userDTO = new UserDTO(user);
                var token = await _securityService.GenerateJwtToken(JsonConvert.SerializeObject(userDTO), userDTO.Role == (int)Role.Admin);

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
                return new ApiResponse<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = _configManager.SeverErrorMessage,
                    Data = null
                };
            }
        }
    }
}
