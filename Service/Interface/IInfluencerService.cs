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
        Task<IEnumerable<Influencer>> GetAllInfluencers();
        Task<Influencer> GetInfluencerById(Guid id);
        Task<IEnumerable<Influencer>> GetTopInfluencer();
        Task<IEnumerable<Influencer>> GetTopInstagramInfluencer();
        Task<IEnumerable<Influencer>> GetTopTiktokInfluencer();
        Task<IEnumerable<Influencer>> GetTopYoutubeInfluencer();
        Task CreateInfluencer(Influencer influencer);
        Task UpdateInfluencer(Influencer influencer);
        Task DeleteInfluencer(Guid id);
    }
}
