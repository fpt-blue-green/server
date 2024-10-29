using BusinessObjects.Models;

namespace Repositories
{
    public interface IBrandRepository
    {
        Task<IEnumerable<Brand>> GetBrands();
        Task<Brand> GetBrandById(Guid brandId);
        Task<Brand> GetBrandByEmail(string email);
        Task<Brand> GetByUserId(Guid id);
        Task UpdateBrand(Brand brand);
        Task CreateBrand(Brand brand);
        Task GetUserById(Guid id);
        Task<Brand> GetBrandWithFavoriteByUserId(Guid userId);
        Task<IEnumerable<Brand>> GetAllExpiredPremiumBrands();
    }
}
