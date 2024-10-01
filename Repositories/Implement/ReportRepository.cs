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

        public Task<IEnumerable<InfluencerReport>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<InfluencerReport> GetById(Guid id)
        {
            using (var context = new PostgresContext())
            {
                var influencerReport = await context.InfluencerReports
                    .SingleOrDefaultAsync(i => i.Id == id);
                return influencerReport!;
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
