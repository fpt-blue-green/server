using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
	public class PackageRepository : SingletonBase<PackageRepository>, IPackageRepository
	{
		public async Task CreateList(List<Package> package)
		{
				await context.Packages.AddRangeAsync(package);
				await context.SaveChangesAsync();	
		}

		public async Task Delete(Guid id)
		{
			var package = await context.Packages.FirstOrDefaultAsync(i => i.Id == id);
			context.Packages.Remove(package);
			await context.SaveChangesAsync();
		}

		public async Task<IEnumerable<Package>> GetAlls()
		{
			var packages = await context.Packages.ToListAsync();
			return packages;
		}

		public async Task<Package> GetById(Guid id)
		{
            var package = await context.Packages.FirstOrDefaultAsync(i => i.Id == id);
            return package!;
        }

		public async Task Update(Package package)
		{
			context.Entry(package).State = EntityState.Modified;
			await context.SaveChangesAsync();
		}
	}
}
