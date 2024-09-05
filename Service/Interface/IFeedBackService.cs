using BusinessObjects.Models;

namespace Service
{
    public interface IFeedBackService
    {
        Task<IEnumerable<Feedback>> GetAllFeedBacks();
        Task<IEnumerable<Feedback>> GetUserFeedBacks(Guid userId);
        Task<double> GetAverageRate(Guid userId);
    }
}
