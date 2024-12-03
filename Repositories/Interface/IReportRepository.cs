using BusinessObjects.Models;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IReportRepository
    {
        Task<IEnumerable<InfluencerReport>> GetAll();
        Task<InfluencerReport> GetById(Guid id);
        Task<InfluencerReport> GetAllById(Guid id);
        Task<IEnumerable<InfluencerReport>> GetReportWithSameType(Guid id);
        Task<IEnumerable<InfluencerReport>> GetInfluencerReportsByInfluencerId(Guid id);
        Task<IEnumerable<InfluencerReport>> GetInfluencerReportsByReporterId(Guid id);
        Task Create(InfluencerReport influencerReport);
        Task Delete(InfluencerReport influencerReport);
        Task UpdateReports(IEnumerable<InfluencerReport> reports);
        Task<IEnumerable<InfluencerReport>> GetInfluencerReportsByInfluencerIdAndReporterId(Guid id, Guid reporterId);
    }
}
