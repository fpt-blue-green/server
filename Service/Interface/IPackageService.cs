

using BusinessObjects;
using BusinessObjects.Models;

namespace Service
{
	public interface IPackageService
	{
		Task<string> CreatePackage(Guid userId,List<PackageDTO> packages);
		Task<string> UpdateInfluencerPackage(Guid userId,Guid packageId, PackageDtoRequest packageDTO);
		Task<List<PackageDTO>> GetInfluPackages(Guid userId);
		Task<PackageDTO> GetInfluPackage( Guid packageId,Guid userId);

	}
}
