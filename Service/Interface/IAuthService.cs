using BusinessObjects.DTOs;
using BusinessObjects.DTOs.AuthDTO;
using BusinessObjects.DTOs.AuthDTOs;
using BusinessObjects.DTOs.UserDTOs;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IAuthService
    {
        Task<UserTokenDTO> Login(LoginDTO loginDTO);
        Task<TokenResponse> RefreshToken(RefreshTokenDTO tokenDTO);
        Task Logout(string token);
        Task<ApiResponse<string>> Register(RegisterDTO registerDTO);
        Task<ApiResponse<string>> ChangePassword(ChangePassDTO changePassDTO, string token);
        Task<bool> Verify(int action, string token);
        Task<ApiResponse<string>> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO);
    }
}
