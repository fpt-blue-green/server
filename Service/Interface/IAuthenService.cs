using BusinessObjects.ModelsDTO;
using BusinessObjects.ModelsDTO.AuthenDTO;

namespace Service.Interface
{
    public interface IAuthenService
    {
        Task<ApiResponse<string>> Login(LoginDTO loginDTO);
    }
}
