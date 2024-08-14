using BusinessObjects.ModelsDTO;
using BusinessObjects.ModelsDTO.AuthenDTO;

namespace Service.Interface
{
    public interface IAuthenService
    {
        Task<ApiResponse<string>> Login(LoginDTO loginDTO);
        Task<ApiResponse<string>> Register(RegisterDTO registerDTO);
        Task<bool> ValidateAuthen(int action, string token);
    }
}
