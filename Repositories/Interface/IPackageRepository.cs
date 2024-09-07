using BusinessObjects.Models;

namespace Repositories
{
	public interface IPackageRepository
	{
		Task<IEnumerable<Package>> GetAlls();
		Task<Package> GetById(Guid id);
		Task Create(Package package);
		Task Update(Package package);
		Task Delete(Guid id);
	}
}
