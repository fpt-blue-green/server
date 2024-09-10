using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class InfluencerRepository : SingletonBase<InfluencerRepository>, IInfluencerRepository
    {
        public InfluencerRepository() { }
        public async Task<IEnumerable<Influencer>> GetAlls()
        {
            var influencers = await context.Influencers
                    .Include(i => i.Channels)
                    .Include(i => i.Tags)
                    .Include(i => i.Packages)
                    .Include(i => i.InfluencerImages)
                    .Include(i => i.User)
                    .ToListAsync();
            return influencers;
        }
        public async Task<Influencer> GetById(Guid id)
        {
            var influencer = await context.Influencers.FirstOrDefaultAsync(i => i.Id == id);
            return influencer;
        }

        public async Task<Influencer> GetByUserId(Guid userId)
        {
            using (var context = new PostgresContext())
            {
                var influencer = await context.Influencers
                                            .Include(i => i.Channels)
                                            .Include(i => i.Tags)
                                            .Include(i => i.Packages)
                                            .Include(i => i.InfluencerImages)
                                            .Include(i => i.User)
                                            .FirstOrDefaultAsync(s => s.UserId == userId);
                return influencer!;
            }
        }

        public async Task<Influencer> GetBySlug(string slug)
        {
            var influencer = await context.Influencers
                                            .Include(i => i.Channels)
                                            .Include(i => i.Tags)
                                            .Include(i => i.Packages)
                                            .Include(i => i.InfluencerImages)
                                            .Include(i => i.User)
                                            .FirstOrDefaultAsync(s => s.Slug == slug);
            return influencer!;
        }

        public async Task Create(Influencer influencer)
        {
            influencer.IsPublish = false;
            await context.Influencers.AddAsync(influencer);
            await context.SaveChangesAsync();
        }
        public async Task Update(Influencer influencer)
        {
            var existingEntity = context.Set<Influencer>().Local
                     .FirstOrDefault(e => e.Id == influencer.Id);

            if (existingEntity != null)
            {
                context.Entry(existingEntity).CurrentValues.SetValues(influencer);
            }
            else
            {
                context.Entry<Influencer>(influencer).State = EntityState.Modified;
            }

            await context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var influencer = await context.Influencers.FirstOrDefaultAsync(i => i.Id == id);
            context.Influencers.Remove(influencer);
            await context.SaveChangesAsync();
        }


        public async Task<List<Tag>> GetTagsByInfluencer(Guid influencerId)
        {
            var influencer = await context.Influencers.Include(i => i.Tags)
                                                .FirstOrDefaultAsync(i => i.Id == influencerId);
            return influencer?.Tags.ToList() ?? new List<Tag>();
        }

        public async Task AddTagToInfluencer(Guid influencerId, Guid tagId)
        {
            var influencer = await context.Influencers.Include(i => i.Tags)
                                              .FirstOrDefaultAsync(i => i.Id == influencerId);

            var tag = await context.Tags.FirstOrDefaultAsync(t => t.Id == tagId);

            if (influencer != null && tag != null)
            {
                influencer.Tags.Add(tag);
                await context.SaveChangesAsync();
            }
        }
		public async Task RemoveTagOfInfluencer(Guid influencerId, Guid tagId)
		{
			var influencer = await context.Influencers.Include(i => i.Tags)
											  .FirstOrDefaultAsync(i => i.Id == influencerId);

			var tag = await context.Tags.FirstOrDefaultAsync(t => t.Id == tagId);

			if (influencer != null && tag != null)
			{
				influencer.Tags.Remove(tag);
				await context.SaveChangesAsync();
			}
		}
	}
}