using BusinessObjects.DTOs;
using BusinessObjects.DTOs.AuthDTO;

namespace Service.Interface
{
    public interface IAuthService
    {
        Task<ApiResponse<string>> Login(LoginDTO loginDTO);
        Task<ApiResponse<string>> Register(RegisterDTO registerDTO);
        Task<ApiResponse<string>> ChangePassword(ChangePassDTO changePassDTO, string token);
        Task<bool> Verify(int action, string token);
        Task<ApiResponse<string>> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO);
    }
}
