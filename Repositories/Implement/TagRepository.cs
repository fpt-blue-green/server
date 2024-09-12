using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class TagRepository : ITagRepository
    {
        public async Task Create(Tag tag)
        {
            using (var context = new PostgresContext())
            {
                await context.Tags.AddAsync(tag);
                await context.SaveChangesAsync();
            }
        }

        public async Task Delete(Guid id)
        {
            using (var context = new PostgresContext())
            {
                var tag = await context.Tags.FirstOrDefaultAsync(i => i.Id == id);
                if (tag != null)
                {
                    context.Tags.Remove(tag);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<IEnumerable<Tag>> GetAlls()
        {
            using (var context = new PostgresContext())
            {
                var tags = await context.Tags.ToListAsync();
                return tags;
            }
        }

        public async Task<Tag> GetById(Guid id)
        {
            using (var context = new PostgresContext())
            {
                var tag = await context.Tags.FirstOrDefaultAsync(i => i.Id == id);
                return tag;
            }
        }

        public async Task Update(Tag tag)
        {
            using (var context = new PostgresContext())
            {
                context.Entry<Tag>(tag).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
    }
}
