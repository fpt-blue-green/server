using BusinessObjects;
using BusinessObjects.Models;

namespace Service
{
    public interface IChannelService
    {
        Task CreateInfluencerChannel(UserDTO user, List<ChannelPlatFormUserNameDTO> channels);
        Task UpdateInfluencerChannel(Channel channel);
        Task<List<ChannelDTO>> GetChannelPlatFormUserNames(UserDTO user);
        Task DeleteInfluencerChannel(Guid id);
    }
}
