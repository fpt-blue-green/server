using BusinessObjects.DTOs.UserDTOs;

namespace Service.Interface
{
    public interface IChannelService
    {
        Task CreateInfluencerChannel(UserDTO user, Dictionary<int, string> channels);
    }
}
