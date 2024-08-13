using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IInfluencerRepository
    {
        public Task<IEnumerable<Influencer>> GetAlls();
        public Task<Influencer> GetById(Guid id);
        public Task Create(Influencer influencer);
        public Task Update(Influencer influencer);
        public Task Delete(Guid id);
    }
}
