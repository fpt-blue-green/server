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

        public async Task Delete(Guid id)
        {
            using (var context = new PostgresContext())
            {
                var feedback = await context.Feedbacks
                    .SingleOrDefaultAsync(i => i.Id == id);

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
