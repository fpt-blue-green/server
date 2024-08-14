using BusinessObjects.Enum;
using BusinessObjects.Models;
using BusinessObjects.ModelsDTO;
using Repositories.Implement;
using Repositories.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implement
{
    public class InfluencerService : IInfluencerService
    {
        private static readonly IInfluencerRepository _repository = new InfluencerRepository();
        public InfluencerService() { }
        public async Task<IEnumerable<Influencer>> GetTopInfluencer()
        {
            var topInflus = (await _repository.GetAlls()).OrderBy(s=> s.RateAverage).Take(10);
            return topInflus.ToList();
        }
        public async Task<IEnumerable<Influencer>> GetTopInstagramInfluencer()
        {
            try
            {
                var topInflus = (await _repository.GetAlls())
                .Where(i => i.Channels.Any(c => c.Type == (int)CChannelType.Instagram))
                .OrderBy(s => s.RateAverage)
                .Take(10);
                return topInflus.ToList();
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<IEnumerable<Influencer>> GetTopTiktokInfluencer()
        {
            try
            {
                var topInflus = (await _repository.GetAlls())
                .Where(i => i.Channels.Any(c => c.Type == (int)CChannelType.Tiktok))
                .OrderBy(s => s.RateAverage)
                .Take(10);
                return topInflus.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<IEnumerable<Influencer>> GetTopYoutubeInfluencer()
        {
            try
            {
                var topInflus = (await _repository.GetAlls())
                .Where(i => i.Channels.Any(c => c.Type == (int)CChannelType.Youtube))
                .OrderBy(s => s.RateAverage)
                .Take(10);
                return topInflus.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task CreateInfluencer(Influencer influencer)
        {
            await _repository.Create(influencer);
        }

        public async Task DeleteInfluencer(Guid id)
        {
            await _repository.Delete(id);
        }

        public async Task<IEnumerable<Influencer>> GetAllInfluencers()
        {
            return await _repository.GetAlls();

        }

        public async Task<Influencer> GetInfluencerById(Guid id)
        {
            return await _repository.GetById(id);
        }

      
        public async Task UpdateInfluencer(Influencer influencer)
        {
            await _repository.Update(influencer);
        }
    }
}
