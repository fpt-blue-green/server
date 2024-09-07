using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class FeedBackRepository : SingletonBase<FeedBackRepository>, IFeedBackRepository
    {
        public FeedBackRepository() { }
        public async Task Create(Feedback feedback)
        {
            await context.Feedbacks.AddAsync(feedback);
            await context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var feedback = await context.Feedbacks.SingleOrDefaultAsync(i => i.Id == id);
            context.Feedbacks.Remove(feedback);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Feedback>> GetAlls()
        {
            var feedbacks = await context.Feedbacks.ToListAsync();
            return feedbacks;
        }

        public async Task<Feedback> GetById(Guid id)
        {

            var feedback = await context.Feedbacks.SingleOrDefaultAsync(i => i.Id == id);
            return feedback;
        }

        public async Task Update(Feedback feedback)
        {
            context.Entry<Feedback>(feedback).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}
