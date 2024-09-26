using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class CampaignImageRepository : ICampaignImageRepository
    {
        public async Task Create(CampaignImage campaignImage)
        {
            using (var context = new PostgresContext())
            {
                await context.CampaignImages.AddAsync(campaignImage);
                await context.SaveChangesAsync();
            }
        }

        public async Task Delete(Guid imageId)
        {
            using (var context = new PostgresContext())
            {
                var image = await context.CampaignImages.FindAsync(imageId);
                if (image != null)
                {
                    context.CampaignImages.Remove(image);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<List<CampaignImage>> GetByCampaignId(Guid campaignId)
        {
            using (var context = new PostgresContext())
            {
                return await context.CampaignImages
                    .Where(image => image.CampaignId == campaignId)
                    .ToListAsync();
            }
        }

        public async Task<int> GetImagesCountByCampaignId(Guid campaignId)
        {
            using (var context = new PostgresContext())
            {
                return await context.CampaignImages
                    .Where(img => img.CampaignId == campaignId)
                    .CountAsync();
            }
        }

        public async Task Update(CampaignImage image)
        {
            using (var context = new PostgresContext())
            {
                var existingImage = await context.CampaignImages.FindAsync(image.Id);

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
    }
}
