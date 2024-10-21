using BusinessObjects;
using BusinessObjects.Models;
using Repositories;

namespace Service
{
    public class BannedUserService : IBannedUserService
    {
        private readonly IUserRepository _userRepository = new UserRepository();
        private readonly IBannedUserRepository  _bannedUserRepository = new BannedUserRepository();

        public async Task<BannedUser> BanUser(User user, BannedUserRequestDTO userRequestDTO, UserDTO userDTO)
        {
            var banUser = new BannedUser
            {
                UserId = user.Id,
                Reason = userRequestDTO.Reason,
                BanDate = DateTime.Now,
                BannedById = userDTO.Id,
                UnbanDate = userRequestDTO.BannedTime == EBanDate.None ? null : DateTime.Now.Add(userRequestDTO.BannedTime.ToTimeSpan()),
            };
            await _bannedUserRepository.CreateBannedUserData(banUser);

            user.IsBanned = userRequestDTO.BannedTime == EBanDate.None ? false : true;
            await _userRepository.UpdateUser(user);

            return banUser;
        }
    }
}
