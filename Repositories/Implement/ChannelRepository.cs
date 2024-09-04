using BusinessObjects.Enum;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Helper;
using Repositories.Interface;

namespace Repositories.Implement
{
    public class ChannelRepository : SingletonBase<ChannelRepository>, IChannelRepository
    {
        public ChannelRepository() { }
        public async Task<IEnumerable<Channel>> GetChannels(Guid influencerId)
        {
            var channels = await context.Channels.Where(c => c.InfluencerId == influencerId).ToListAsync();
            return channels;
        }

        public async Task<IEnumerable<Channel>> GetChannel(Guid influencerId, EPlatform platform)
        {
            var channels = await context.Channels.Where(c => c.InfluencerId == influencerId && c.Type == (int)platform).ToListAsync();
            return channels;
        }

        public async Task CreateChannel(Channel channel)
        {
            channel.CreatedAt = DateTime.UtcNow;
            context.Channels.Add(channel);
            await context.SaveChangesAsync();
        }

        public async Task UpdateChannel(Channel channel)
        {
            channel.ModifiedAt = DateTime.UtcNow;
            var localChannel = context.Set<Channel>()
                                   .Local
                                   .FirstOrDefault(entry => entry.Id.Equals(channel.Id) && entry.Type == channel.Type);
            if (localChannel != null)
            {
                context.Entry(localChannel).State = EntityState.Detached;
            }

            context.Entry(channel).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}
