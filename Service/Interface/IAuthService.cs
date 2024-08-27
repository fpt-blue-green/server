using BusinessObjects.DTOs;
using BusinessObjects.DTOs.AuthDTO;
using BusinessObjects.DTOs.AuthDTOs;
using BusinessObjects.DTOs.UserDTOs;

namespace Service.Interface
{
    public interface IAuthService
    {
        Task<ApiResponse<UserDTO>> Login(LoginDTO loginDTO);
        Task<ApiResponse<TokenResponse>> RefreshToken(RefreshTokenDTO tokenDTO);
        Task Logout(string token);
        Task<ApiResponse<string>> Register(RegisterDTO registerDTO);
        Task<ApiResponse<string>> ChangePassword(ChangePassDTO changePassDTO, string token);
        Task<bool> Verify(int action, string token);
        Task<ApiResponse<string>> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO);
    }
}
