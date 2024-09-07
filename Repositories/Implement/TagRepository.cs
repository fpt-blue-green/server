using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class TagRepository : SingletonBase<TagRepository>, ITagRepository
    {
        public TagRepository()
        {

        }
        public async Task Create(Tag tag)
        {
            await context.Tags.AddAsync(tag);
            await context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var tag = await context.Tags.FirstOrDefaultAsync(i => i.Id == id);
            context.Tags.Remove(tag);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Tag>> GetAlls()
        {
            var tags = await context.Tags.ToListAsync();
            return tags;
        }

        public async Task<Tag> GetById(Guid id)
        {
            var tag = await context.Tags.FirstOrDefaultAsync(i => i.Id == id);
            return tag;
        }

        public async Task Update(Tag tag)
        {
            context.Entry<Tag>(tag).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}
