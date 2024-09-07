using BusinessObjects.Models;

namespace Repositories
{
    public interface IFeedBackRepository
    {
        Task<IEnumerable<Feedback>> GetAlls();
        Task<Feedback> GetById(Guid id);
        Task Create(Feedback feedback);
        Task Update(Feedback feedback);
        Task Delete(Guid id);
    }
}
