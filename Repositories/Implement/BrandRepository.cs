using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;

namespace Repositories.Implement
{
    public class BrandRepository : IBrandRepository
    {
        public async Task CreateBrand(Brand brand)
        {
            using (var context = new PostgresContext())
            {
                await context.Brands.AddAsync(brand);
                await context.SaveChangesAsync();
            }
        }

        public Task<Brand> GetBrandByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<Brand> GetBrandById(Guid brandId)
        {
            using (var context = new PostgresContext())
            {
                var brand = await context.Brands
                    .FirstOrDefaultAsync(u => u.Id == brandId);
                return brand!;
            }
        }

        public async Task<IEnumerable<Brand>> GetBrands()
        {
            using (var context = new PostgresContext())
            {
                var brands = await context.Brands.ToListAsync();
                return brands;
            }
        }

        public async Task<Brand> GetByUserId(Guid id)
        {
            using (var context = new PostgresContext())
            {
                var brand = await context.Brands
                                            .Include(i => i.Campaigns)
                                            .Include(i => i.User)
                                            .FirstOrDefaultAsync(s => s.UserId == id);
                return brand!;
            }
        }

        public Task GetUserById(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateBrand(Brand brand)
        {
            using (var context = new PostgresContext())
            {
                var existingEntity = context.Set<Brand>().Local
                 .FirstOrDefault(e => e.Id == brand.Id);

                if (existingEntity != null)
                {
                    context.Entry(existingEntity).CurrentValues.SetValues(brand);
                }
                else
                {
                    context.Entry<Brand>(brand).State = EntityState.Modified;
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
