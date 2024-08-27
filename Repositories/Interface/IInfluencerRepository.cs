using BusinessObjects.DTOs.InfluencerDTOs;
using BusinessObjects.Models;

namespace Repositories.Interface
{
    public interface IInfluencerRepository
    {
        Task<IEnumerable<Influencer>> GetAlls();
        Task<Influencer> GetById(Guid id);
        Task<Influencer> GetByUserId(Guid userId);
        Task Create(Influencer influencer);
        Task Update(Influencer influencer);
        Task Delete(Guid id);
    }
}
