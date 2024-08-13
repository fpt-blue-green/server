using BusinessObjects.Models;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IInfluencerService
    {
        public Task<IEnumerable<Influencer>> GetAllInfluencers();
        public Task<Influencer> GetInfluencerById(Guid id);
        public Task CreateInfluencer(Influencer influencer);
        public Task UpdateInfluencer(Influencer influencer);
        public Task DeleteInfluencer(Guid id);
    }
}
