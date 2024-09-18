using BusinessObjects;
using Microsoft.AspNetCore.Http;

namespace Service.Interface
{
    public interface IBrandService
    {
        Task<string> UploadCoverAsync(IFormFile file, string folder, UserDTO user);
        Task<string> CreateOrUpdateBrand(BrandRequestDTO brandRequestDTO, UserDTO user);
        Task<BrandDTO> GetBrandByUserId(Guid userId);

    }
}
