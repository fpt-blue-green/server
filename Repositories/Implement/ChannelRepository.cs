using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class ChannelRepository : IChannelRepository
    {
        public async Task<IEnumerable<Channel>> GetChannels(Guid influencerId)
        {
            using (var context = new PostgresContext())
            {
                var channels = await context.Channels
                    .Where(c => c.InfluencerId == influencerId)
                    .ToListAsync();
                return channels;
            }
        }

        public async Task<IEnumerable<Channel>> GetChannel(Guid influencerId, EPlatform platform)
        {
            using (var context = new PostgresContext())
            {
                var channels = await context.Channels
                    .Where(c => c.InfluencerId == influencerId && c.Platform == (int)platform)
                    .ToListAsync();
                return channels;
            }
        }

        public async Task CreateChannel(Channel channel)
        {
            using (var context = new PostgresContext())
            {
                context.Channels.Add(channel);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateChannel(Channel channel)
        {
            using (var context = new PostgresContext())
            {
                var localChannel = context.Set<Channel>()
                    .Local
                    .FirstOrDefault(entry => entry.Id.Equals(channel.Id) && entry.Platform == channel.Platform);
                if (localChannel != null)
                {
                    context.Entry(localChannel).State = EntityState.Detached;
                }

                context.Entry(channel).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteChannel(Guid id)
        {
            using (var context = new PostgresContext())
            {
                var channel = await context.Channels
                    .FirstOrDefaultAsync(i => i.Id == id);
                if (channel != null)
                {
                    context.Channels.Remove(channel);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
