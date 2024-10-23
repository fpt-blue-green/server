using BusinessObjects;
using BusinessObjects.Models;

namespace Service
{
    public interface IBannedUserService
    {
        Task<BannedUser> BanUserBaseOnReport(User user, BannedUserRequestDTO userRequestDTO, UserDTO userDTO);
        Task BanUser(Guid userId, BannedUserRequestDTO userRequestDTO, UserDTO userDTO);
        Task UnBanUser(Guid userId, BannedUserRequestDTO userRequestDTO, UserDTO userDTO);
        Task<IEnumerable<BannedUserDTO>> GetBannedUsers();
    }
}
