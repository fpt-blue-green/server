using BusinessObjects.DTOs.InfluencerDTO;
using BusinessObjects.DTOs.UserDTOs;
using BusinessObjects.Models;

namespace Service.Interface
{
    public interface IChannelService
    {
        Task CreateInfluencerChannel(UserDTO user, List<ChannelPlatFormUserNameDTO> channels);
        Task UpdateInfluencerChannel(Channel channel);
        Task<List<ChannelPlatFormUserNameDTO>> GetChannelPlatFormUserNames(UserDTO user);
    }
}
