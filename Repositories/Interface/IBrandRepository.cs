using BusinessObjects.Models;

namespace Repositories.Interface
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
    }
}
