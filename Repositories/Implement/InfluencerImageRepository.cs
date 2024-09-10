using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class InfluencerImageRepository : SingletonBase<InfluencerImageRepository>, IInfluencerImageRepository
    {
        public InfluencerImageRepository() { }

        public async Task Create(InfluencerImage influencerImage)
        {
            await context.InfluencerImages.AddAsync(influencerImage);
            await context.SaveChangesAsync();
        }

        public async Task<List<InfluencerImage>> GetByInfluencerId(Guid influencerId)
        {
            return await context.InfluencerImages
                                 .Where(image => image.InfluencerId == influencerId)
                                 .ToListAsync();
        }

        public async Task Delete(Guid imageId)
        {
            var image = await context.InfluencerImages.FindAsync(imageId);
            if (image != null)
            {
                context.InfluencerImages.Remove(image);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteByUrl(string url)
        {
            var image = await context.InfluencerImages.FirstOrDefaultAsync(x => x.Url == url);
            if (image != null)
            {
                context.InfluencerImages.Remove(image);
                await context.SaveChangesAsync();
            }
        }

        public async Task Update(InfluencerImage image)
        {
            var existingImage = await context.Set<InfluencerImage>().FindAsync(image.Id);

            if (existingImage == null)
            {
                throw new InvalidOperationException("Image not found.");
            }

            existingImage.Url = image.Url;
            existingImage.ModifiedAt = image.ModifiedAt;

            context.Entry(existingImage).State = EntityState.Modified;

            await context.SaveChangesAsync();
        }

        public async Task<int> GetImagesCountByInfluencerId(Guid influencerId)
        {
            return await context.InfluencerImages
                .Where(img => img.InfluencerId == influencerId)
                .CountAsync();
        }
    }
}
