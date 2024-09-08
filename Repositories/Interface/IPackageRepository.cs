using BusinessObjects.Models;

namespace Repositories
{
	public interface IPackageRepository
	{
		Task<IEnumerable<Package>> GetAlls();
		Task<Package> GetById(Guid id);
		Task CreateList(List<Package> packages);
		Task Update(Package package);
		Task Delete(Guid id);
	}
}
