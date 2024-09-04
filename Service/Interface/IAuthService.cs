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
        Task<string> Register(RegisterDTO registerDTO);
        Task<string> ChangePassword(ChangePassDTO changePassDTO, UserDTO user);
        Task<bool> Verify(VerifyDTO data);
        Task<string > ForgotPassword(ForgotPasswordDTO forgotPasswordDTO);
    }
}
