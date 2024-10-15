using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class FeedBackRepository : IFeedBackRepository
    {
        public async Task Create(Feedback feedback)
        {
            using (var context = new PostgresContext())
            {
                await context.Feedbacks.AddAsync(feedback);
                await context.SaveChangesAsync();
            }
        }

        public async Task Delete(Feedback feedback)
        {
            using (var context = new PostgresContext())
            {

                if (feedback != null)
                {
                    context.Feedbacks.Remove(feedback);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<IEnumerable<Feedback>> GetAlls()
        {
            using (var context = new PostgresContext())
            {
                var feedbacks = await context.Feedbacks.Include(f => f.User).ToListAsync();
                return feedbacks;
            }
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacksByInfluencerId(Guid influencerId)
        {
            using (var context = new PostgresContext())
            {
                var feedbacks = await context.Feedbacks.Include(f => f.User).Where(f => f.InfluencerId == influencerId).ToListAsync();
                return feedbacks;
            }
        }

        public async Task<(Influencer Influencer, Feedback? ExistingFeedback, List<Feedback> FeedbacksForInfluencer)> GetInfluencerAndFeedback(Guid userId, Guid influencerId)
        {
            using (var context = new PostgresContext())
            {
                var feedbacksForInfluencer = await context.Feedbacks
                                                    .Where(f => f.InfluencerId == influencerId)
                                                    .ToListAsync();

                var existingFeedback = feedbacksForInfluencer.FirstOrDefault(f => f.UserId == userId);
                var influencer = await context.Influencers
                                       .Include(i => i.User)
                                       .FirstOrDefaultAsync(i => i.Id == influencerId);

                return (influencer, existingFeedback, feedbacksForInfluencer)!;
            }
        }

        public async Task<Feedback> GetById(Guid id)
        {
            using (var context = new PostgresContext())
            {
                var feedback = await context.Feedbacks
                    .SingleOrDefaultAsync(i => i.Id == id);
                return feedback;
            }
        }

        public async Task Update(Feedback feedback)
        {
            using (var context = new PostgresContext())
            {
                var localFeedback = context.Set<Feedback>()
                    .Local
                    .FirstOrDefault(entry => entry.Id.Equals(feedback.Id));

                if (localFeedback != null)
                {
                    context.Entry(localFeedback).State = EntityState.Detached;
                }

                context.Entry(feedback).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
    }
}
