using BusinessObjects;
using BusinessObjects.Models;

namespace Service
{
    public interface IChannelService
    {
        Task CreateInfluencerChannel(UserDTO user, List<ChannelPlatFormUserNameDTO> channels);
        Task UpdateInfluencerChannel(Channel channel);
        Task<List<ChannelPlatFormUserNameDTO>> GetChannelPlatFormUserNames(UserDTO user);
    }
}
