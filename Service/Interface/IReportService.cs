using BusinessObjects;
using BusinessObjects.Models;

namespace Service
{
    public interface IReportService
    {
        Task<IEnumerable<InfluencerReport>> GetInfluencerReports();
        Task<IEnumerable<InfluencerReport>> GetInfluencerReportsByInfluencerId(Guid influencerId);
        Task<InfluencerReport> GetInfluencerReportById(Guid id);
        Task CreateInfluencerReport(Guid influencerId, ReportRequestDTO reportRequestDTO, UserDTO userDTO);
        Task DeleteInfluencerReport(Guid id, UserDTO userDTO);
    }
}
