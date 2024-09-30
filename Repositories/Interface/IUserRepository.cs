using BusinessObjects;
using BusinessObjects.Models;

namespace Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUserById(Guid userId);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByLoginDTO(LoginDTO loginDTO);
        Task UpdateUser(User user);
        Task CreateUser(User user);
    }
}
