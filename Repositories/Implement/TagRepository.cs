using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Helper;
using Repositories.Interface;

namespace Repositories.Implement
{
    public class TagRepository : SingletonBase<TagRepository>, ITagRepository
    {
        public TagRepository()
        {

        }
        public async Task Create(Tag tag)
        {
            try
            {

                await context.Tags.AddAsync(tag);
                await context.SaveChangesAsync();

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
                var tag = await context.Tags.SingleOrDefaultAsync(i => i.Id == id);
                context.Tags.Remove(tag);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Tag>> GetAlls()
        {
            var tags = new List<Tag>();
            try
            {
                tags = await context.Tags
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return tags;
        }

        public async Task<Tag> GetById(Guid id)
        {
            var tag = new Tag();
            try
            {
                tag = await context.Tags.SingleOrDefaultAsync(i => i.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return tag;
        }

        public async Task Update(Tag tag)
        {
            try
            {
                context.Entry<Tag>(tag).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
