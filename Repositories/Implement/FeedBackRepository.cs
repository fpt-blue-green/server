using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implement
{
    public class FeedBackRepository : IFeedBackRepository
    {
        public FeedBackRepository() { }
        public async Task Create(Feedback feedback)
        {
            try
            {
                using (var context = new PostgresContext())
                {
                    await context.Feedbacks.AddAsync(feedback);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task Delete(Guid id)
        {
            try
            {
                using (var context = new PostgresContext())
                {
                    var feedback = await context.Feedbacks.SingleOrDefaultAsync(i => i.Id == id);
                    context.Feedbacks.Remove(feedback);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Feedback>> GetAlls()
        {
            var feedbacks = new List<Feedback>();
            try
            {
                using (var context = new PostgresContext())
                {
                    feedbacks = await context.Feedbacks.ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return feedbacks;
        }

        public async Task<Feedback> GetById(Guid id)
        {

            var feedback = new Feedback();
            try
            {
                using (var context = new PostgresContext())
                {
                    feedback = await context.Feedbacks.SingleOrDefaultAsync(i => i.Id == id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return feedback;
        }

        public async Task Update(Feedback feedback)
        {
            try
            {
                using (var context = new PostgresContext())
                {
                    context.Entry<Feedback>(feedback).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
