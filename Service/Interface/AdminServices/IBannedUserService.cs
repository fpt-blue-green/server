using BusinessObjects;
using BusinessObjects.Models;

namespace Service
{
    public interface IBannedUserService
    {
        Task<BannedUser> BanUser(User user, BannedUserRequestDTO userRequestDTO, UserDTO userDTO);
    }
}
