using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;

namespace Repositories.Implement
{
    public class ReportRepository : IReportRepository
    {
        public async Task Create(InfluencerReport influencerReport)
        {
            using (var context = new PostgresContext())
            {
                await context.InfluencerReports.AddAsync(influencerReport);
                await context.SaveChangesAsync();
            }
        }

        public async Task Delete(InfluencerReport influencerReport)
        {
            using (var context = new PostgresContext())
            {
                context.InfluencerReports.Remove(influencerReport);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateReports(IEnumerable<InfluencerReport> reports)
        {
            using (var context = new PostgresContext())
            {
                context.InfluencerReports.UpdateRange(reports); 
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<InfluencerReport>> GetAll()
        {
            using (var context = new PostgresContext())
            {
                var result = await context.InfluencerReports
                    .Include(j => j.Influencer)
                    .Include(j =>j.Reporter).IgnoreQueryFilters().ToListAsync();
                return result;
            }
        }

        public async Task<InfluencerReport> GetAllById(Guid id)
        {
            using (var context = new PostgresContext())
            {
                var influencerReport = await context.InfluencerReports
                    .Include(x => x.Influencer)
                    .Include(x => x.Reporter)
                    .SingleOrDefaultAsync(i => i.Id == id);
                return influencerReport!;
            }
        }

        public async Task<InfluencerReport> GetById(Guid id)
        {
            using (var context = new PostgresContext())
            {
                var influencerReport = await context.InfluencerReports
                    .Where(x => x.ReportStatus == (int)EReportStatus.Pending)
                    .Include(x => x.Influencer)
                    .Include(x => x.Reporter)
                    .SingleOrDefaultAsync(i => i.Id == id);
                return influencerReport!;
            }
        }
        public async Task<IEnumerable<InfluencerReport>> GetReportWithSameType(Guid id)
        {
            using (var context = new PostgresContext())
            {
                // Truy vấn lấy cả báo cáo chính và các báo cáo cùng loại
                var reports = await context.InfluencerReports
                    .Include(x => x.Influencer).ThenInclude(i => i.User).ThenInclude(i => i.BannedUserUsers)
                    .Include(x => x.Reporter)
                    .Where(i => i.Id == id  || (i.InfluencerId == (context.InfluencerReports.Where(r => r.Id == id).Select(r => r.InfluencerId).FirstOrDefault())
                                                 && i.Reason == (context.InfluencerReports .Where(r => r.Id == id).Select(r => r.Reason).FirstOrDefault())))
                    .ToListAsync();

                return reports;
            }
        }


        public async Task<IEnumerable<InfluencerReport>> GetInfluencerReportsByInfluencerId(Guid id)
        {
            using (var context = new PostgresContext())
            {
                var influencerReports = await context.InfluencerReports
                    .Where(i => i.InfluencerId == id)
                    .ToListAsync();
                return influencerReports;
            }
        }

        public async Task<IEnumerable<InfluencerReport>> GetInfluencerReportsByInfluencerIdAndReporterId(Guid id, Guid reporterId)
        {
            using (var context = new PostgresContext())
            {
                var influencerReports = await context.InfluencerReports
                    .Where(i => i.InfluencerId == id && i.ReporterId == reporterId)
                    .ToListAsync();
                return influencerReports;
            }
        }

        public async Task<IEnumerable<InfluencerReport>> GetInfluencerReportsByReporterId(Guid id)
        {
            using (var context = new PostgresContext())
            {
                var influencerReports = await context.InfluencerReports
                    .Where(i => i.ReporterId == id).ToListAsync();
                return influencerReports;
            }
        }
    }
}
