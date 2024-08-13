using BusinessObjects.Models;
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
