using BusinessObjects.Models;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IReportRepository
    {
        Task<IEnumerable<InfluencerReport>> GetAll();
        Task<InfluencerReport> GetById(Guid id);
        Task<IEnumerable<InfluencerReport>> GetInfluencerReportsByInfluencerId(Guid id);
        Task<IEnumerable<InfluencerReport>> GetInfluencerReportsByReporterId(Guid id);
        Task Create(InfluencerReport influencerReport);
        Task Delete(InfluencerReport influencerReport);
    }
}
