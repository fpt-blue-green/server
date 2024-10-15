using BusinessObjects.Models;

namespace Repositories
{
    public interface IFeedBackRepository
    {
        Task<(Influencer Influencer, Feedback? ExistingFeedback, List<Feedback> FeedbacksForInfluencer)> GetInfluencerAndFeedback(Guid userId, Guid influencerId);
        Task<IEnumerable<Feedback>> GetAlls();
        Task<IEnumerable<Feedback>> GetFeedbacksByInfluencerId(Guid influencerId);
        Task<Feedback> GetById(Guid id);
        Task Create(Feedback feedback);
        Task Update(Feedback feedback);
        Task Delete(Feedback feedback);
    }
}
