using BusinessObjects.Models;

namespace Repositories
{
    public interface ITagRepository
    {
        Task<IEnumerable<Tag>> GetAlls();
        Task<Tag> GetById(Guid id);
        Task Create(Tag tag);
        Task Update(Tag tag);
        Task Delete(Tag tag);
    }
}
