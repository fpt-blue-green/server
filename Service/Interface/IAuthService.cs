using BusinessObjects.DTOs;
using BusinessObjects.DTOs.AuthDTOs;

namespace Service.Interface
{
    public interface IAuthService
    {
        Task<ApiResponse<string>> Login(LoginDTO loginDTO);
        Task<ApiResponse<string>> Register(RegisterDTO registerDTO);
        Task<bool> Verify(int action, string token);
    }
}
