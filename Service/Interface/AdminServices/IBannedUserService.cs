using BusinessObjects;
using BusinessObjects.Models;

namespace Service
{
    public interface IBannedUserService
    {
        Task<BannedUser> BanUserBaseOnReport(User user, BannedUserRequestDTO userRequestDTO, UserDTO userDTO);
        Task BanUser(Guid userId, BannedUserRequestDTO userRequestDTO, UserDTO userDTO);
        Task UnBanUser(Guid userId, BannedUserRequestDTO userRequestDTO, UserDTO userDTO);
        Task<FilterListResponse<BannedUserDTO>> GetBannedUsers(FilterDTO filter);
        Task<(byte[] fileContent, string fileName)> GetDataFile();
        Task<BannedUserDTO> GetBannedUserById(Guid id);
    }
}
