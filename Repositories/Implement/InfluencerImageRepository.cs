using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class InfluencerImageRepository : IInfluencerImageRepository
    {
        public async Task Create(InfluencerImage influencerImage)
        {
            using (var context = new PostgresContext())
            {
                await context.InfluencerImages.AddAsync(influencerImage);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<InfluencerImage>> GetByInfluencerId(Guid influencerId)
        {
            using (var context = new PostgresContext())
            {
                return await context.InfluencerImages
                    .Where(image => image.InfluencerId == influencerId)
                    .ToListAsync();
            }
        }

        public async Task Delete(Guid imageId)
        {
            using (var context = new PostgresContext())
            {
                var image = await context.InfluencerImages.FindAsync(imageId);
                if (image != null)
                {
                    context.InfluencerImages.Remove(image);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteByUrl(string url)
        {
            using (var context = new PostgresContext())
            {
                var image = await context.InfluencerImages
                    .FirstOrDefaultAsync(x => x.Url == url);
                if (image != null)
                {
                    context.InfluencerImages.Remove(image);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task Update(InfluencerImage image)
        {
            using (var context = new PostgresContext())
            {
                var existingImage = await context.InfluencerImages.FindAsync(image.Id);

                if (existingImage == null)
                {
                    throw new InvalidOperationException("Không tìm thấy hình ảnh.");
                }

                existingImage.Url = image.Url;
                existingImage.ModifiedAt = image.ModifiedAt;

                context.Entry(existingImage).State = EntityState.Modified;

                await context.SaveChangesAsync();
            }
        }

        public async Task<int> GetImagesCountByInfluencerId(Guid influencerId)
        {
            using (var context = new PostgresContext())
            {
                return await context.InfluencerImages
                    .Where(img => img.InfluencerId == influencerId)
                    .CountAsync();
            }
        }
    }
}
