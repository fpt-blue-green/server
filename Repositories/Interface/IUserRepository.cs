using BusinessObjects;
using BusinessObjects.Models;

namespace Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsers();
        Task<IEnumerable<User>> GetUsersIgnoreFilter();
        Task<User> GetUserById(Guid userId);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByLoginDTO(LoginDTO loginDTO);
        Task UpdateUser(User user);
        Task CreateUser(User user);
        Task<User> GetUserByCampaignId(Guid campaignId);
        Task<User> GetUserByInfluencerId(Guid influencerId);
        Task DeleteUser(Guid userId);
        Task<IEnumerable<User>> GetInfluencerUsersWithPaymentHistory();
        Task<IEnumerable<User>> GetBrandUsersWithPaymentHistory();
        Task<IEnumerable<UserPaymentDTO>> GetUserPayments(Guid userID);
    }
}
