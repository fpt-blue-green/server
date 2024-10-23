using BusinessObjects;

namespace Service
{
    public interface IAuthService
    {
        Task<UserTokenDTO> Login(LoginDTO loginDTO, string userAgent);
        Task<TokenResponseDTO> RefreshToken(RefreshTokenDTO tokenDTO, string userAgent);
        Task Logout(string userAgent, string token);
        Task<string> Register(RegisterDTO registerDTO);
        Task<string> ChangePassword(ChangePassDTO changePassDTO, UserDTO user);
        Task<bool> Verify(VerifyDTO data);
        Task<string > ForgotPassword(ForgotPasswordDTO forgotPasswordDTO);
        Task RegisterWithThirdParty(RegisterThirdPartyDTO registerDTO);
    }
}
