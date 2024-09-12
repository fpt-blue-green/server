using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class PackageRepository : IPackageRepository
    {
        public async Task CreateList(List<Package> packages)
        {
            using (var context = new PostgresContext())
            {
                await context.Packages.AddRangeAsync(packages);
                await context.SaveChangesAsync();
            }
        }

        public async Task Delete(Guid id)
        {
            using (var context = new PostgresContext())
            {
                var package = await context.Packages.FirstOrDefaultAsync(i => i.Id == id);
                if (package != null)
                {
                    context.Packages.Remove(package);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task<IEnumerable<Package>> GetAlls()
        {
            using (var context = new PostgresContext())
            {
                var packages = await context.Packages.ToListAsync();
                return packages;
            }
        }

        public async Task<Package> GetById(Guid id)
        {
            using (var context = new PostgresContext())
            {
                var package = await context.Packages.FirstOrDefaultAsync(i => i.Id == id);
                return package!;
            }
        }

        public async Task Update(Package package)
        {
            using (var context = new PostgresContext())
            {
                context.Entry(package).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
    }
}
