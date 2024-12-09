using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class EmbeddingRepository : IEmbeddingRepository
    {
        public async Task Create(Embedding embedding)
        {
            using (var context = new PostgresContext())
            {
                await context.Embeddings.AddAsync(embedding);
                await context.SaveChangesAsync();
            }
        }

        public async Task<Embedding> GetEmbeddingByInfluencerId(Guid id)
        {
            using (var context = new PostgresContext())
            {
                var result = await context.Embeddings.FirstOrDefaultAsync(x => x.InfluencerId == id);
                return result!;
            }
        }

        public async Task<Embedding> GetEmbeddingByCampaignId(Guid id)
        {
            using (var context = new PostgresContext())
            {
                var result = await context.Embeddings.FirstOrDefaultAsync(x => x.CampaignId == id);
                return result!;
            }
        }

        public async Task Update(Embedding embedding)
        {
            using (var context = new PostgresContext())
            {
                context.Entry<Embedding>(embedding).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
    }
}
