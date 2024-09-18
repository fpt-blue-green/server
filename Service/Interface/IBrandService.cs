using BusinessObjects;
using Microsoft.AspNetCore.Http;

namespace Service.Interface
{
    public interface IBrandService
    {
        Task<string> UploadCoverAsync(IFormFile file, string folder, UserDTO user);
    }
}
