using BusinessObjects.Enum;
using BusinessObjects.Models;

namespace Repositories.Interface
{
    public interface IChannelRepository
    {
        Task<IEnumerable<Channel>> GetChannels(Guid influencerId);
        Task<IEnumerable<Channel>> GetChannel(Guid influencerId, EPlatform platform);
        Task CreateChannel(Channel channel);
        Task UpdateChannel(Channel channel);
    }
}
