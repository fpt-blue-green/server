using BusinessObjects;

namespace Service
{
    public interface IAuthService
    {
        Task<UserTokenDTO> Login(LoginDTO loginDTO);
        Task<TokenResponseDTO> RefreshToken(RefreshTokenDTO tokenDTO);
        Task Logout(string token);
        Task<string> Register(RegisterDTO registerDTO);
        Task<string> ChangePassword(ChangePassDTO changePassDTO, UserDTO user);
        Task<bool> Verify(VerifyDTO data);
        Task<string > ForgotPassword(ForgotPasswordDTO forgotPasswordDTO);
    }
}
