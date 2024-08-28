using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Helper;
using Repositories.Interface;

namespace Repositories.Implement
{
    public class InfluencerRepository : SingletonBase<InfluencerRepository>, IInfluencerRepository
    {
        public InfluencerRepository() { }
        public async Task<IEnumerable<Influencer>> GetAlls()
        {
            var influencers = new List<Influencer>();
            try
            {
                influencers = await context.Influencers
                    .Include(i => i.Channels)
                    .Include(i => i.Tags)
                    .Include(i => i.Packages)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return influencers;
        }
        public async Task<Influencer> GetById(Guid id)
        {
            var influencer = new Influencer();
            try
            {

                influencer = await context.Influencers.SingleOrDefaultAsync(i => i.Id == id);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return influencer;
        }

        public async Task<Influencer> GetByUserId(Guid userId)
        {
            var influencer = new Influencer();
            try
            {
                influencer = await context.Influencers.SingleOrDefaultAsync(s => s.UserId == userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return influencer;
        }

        public async Task Create(Influencer influencer)
        {
            try
            {
                await context.Influencers.AddAsync(influencer);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task Update(Influencer influencer)
        {
            try
            {

                context.Entry<Influencer>(influencer).State = EntityState.Modified;
                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task Delete(Guid id)
        {
            try
            {
                var influencer = await context.Influencers.SingleOrDefaultAsync(i => i.Id == id);
                context.Influencers.Remove(influencer);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Influencer> GetByUserId(Guid id)
        {
            var influencer = new Influencer();
            try
            {
                influencer = await context.Influencers.FirstOrDefaultAsync(i => i.UserId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return influencer;
        }

        public async Task<List<Tag>> GetTagsByInfluencer(Guid influencerId)
        {
            var influencer = await context.Influencers.Include(i => i.Tags)
                                                .FirstOrDefaultAsync(i => i.Id == influencerId);
            return influencer?.Tags.ToList() ?? new List<Tag>();
        }

        /*public async Task AddTagToInfluencer(Guid influencerId, Guid tagId)
        {
            var influencer = await context.Influencers.Include(i => i.Tags) 
                                              .FirstOrDefaultAsync(i => i.Id == influencerId);

            var tag = await context.Tags.FirstOrDefaultAsync(t => t.Id == tagId);


            if (influencer != null && tag != null)
            {
                influencer.Tags.Add(tag);
                await context.SaveChangesAsync();
            }
        }*/
/*
        public async Task RemoveTagFromInfluencer(Guid influencerId, Guid tagId)
        {
            var influencer = await context.Influencers.Include(i => i.Tags)
                                                .FirstOrDefaultAsync(i => i.Id == influencerId);

            if (influencer != null)
            {
                var tagToRemove = influencer.Tags.FirstOrDefault(t => t.Id == tagId);

                if (tagToRemove != null)
                {
                    influencer.Tags.Remove(tagToRemove);
                    await context.SaveChangesAsync();
                }
            }
        }*/

        public async Task UpdateTagsForInfluencer(Guid influencerId, List<Guid> tagIds)
        {
            var influencer = await context.Influencers.Include(i => i.Tags)
                                                .FirstOrDefaultAsync(i => i.Id == influencerId);
            if (influencer != null)
            {
                influencer.Tags.Clear();
                var newTags = await context.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync();
                foreach (var tag in newTags)
                {
                    influencer.Tags.Add(tag);
                }
                await context.SaveChangesAsync();
            }
        }
    }
}