using BusinessObjects.Models;
using Repositories.Implement;
using Repositories.Interface;
using Service.Interface;

namespace Service.Implement
{
    public class FeedBackService : IFeedBackService
    {
        private static readonly IFeedBackRepository _repository = new FeedBackRepository();
        public async Task<double> GetAverageRate(Guid userId)
        {
            var userFeedbacks = (await GetUserFeedBacks(userId)).ToList();
            double averageRate = 0;
            if (userFeedbacks.Any())
            {
                var validRates = userFeedbacks
                .Where(f => f.Rating.HasValue)
                .Select(f => f.Rating.Value);
                averageRate = validRates.Average();
            }
            return averageRate;
        }

        public Task<IEnumerable<Feedback>> GetAllFeedBacks()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Feedback>> GetUserFeedBacks(Guid userId)
        {
            var feedbacks = (await _repository.GetAlls()).Where(s => s.UserId == userId).ToList();
            return feedbacks;
        }
    }
}
