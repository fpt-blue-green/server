using BusinessObjects.Models;

namespace Repositories
{
    public interface IEmbeddingRepository
    {
        Task Create(Embedding embedding);
        Task<Embedding> GetEmbeddingByInfluencerId(Guid id);
        Task Update(Embedding embedding);
    }
}
