using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Helper;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    .Include(i => i.InfluencerTags)
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
    }
}