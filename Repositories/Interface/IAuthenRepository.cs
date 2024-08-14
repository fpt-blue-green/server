using BusinessObjects.Models;
using BusinessObjects.ModelsDTO.AuthenDTO;

namespace Repositories.Interface
{
    public interface IAuthenRepository
    {
        Task<List<User>> GetUsers();
        Task<User> GetUserById(Guid userId);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByLoginDTO(LoginDTO loginDTO);
        Task UpdateUser(User user);
        Task CreateUser(User user);
    }
}
