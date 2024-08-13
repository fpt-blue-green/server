using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implement
{
    public class InfluencerRepository : IInfluencerRepository
    {
        public InfluencerRepository() { }
        public async Task<IEnumerable<Influencer>> GetAlls()
        {
            var influencers = new List<Influencer>();
            try
            {
                using (var context = new PostgresContext())
                {
                    influencers = await context.Influencers.ToListAsync();
                }
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
                using (var context = new PostgresContext())
                {
                    influencer = await context.Influencers.SingleOrDefaultAsync(i => i.Id == id);
                }
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
                using (var context = new PostgresContext())
                {
                    await context.Influencers.AddAsync(influencer);
                    await context.SaveChangesAsync();
                }
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
                using (var context = new PostgresContext())
                {
                    context.Entry<Influencer>(influencer).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<Influencer> Delete(Guid id)
        {
            try
            {
                using (var context = new PostgresContext())
                {
                    var influencer = await context.Influencers.SingleOrDefaultAsync(i => i.Id == id);

                    if (influencer == null)
                    {
                        throw new InvalidOperationException("Influencer not found");
                    }
                    context.Influencers.Remove(influencer);
                    await context.SaveChangesAsync();
                    return influencer;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the influencer: " + ex.Message, ex);
            }
        }
    }
}
