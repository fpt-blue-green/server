
using BusinessObjects;
using Microsoft.AspNetCore.Http;

namespace Service
{
    public interface IUserService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder, UserDTO user);
        Task<List<string>> UploadContentImages(List<string> imageIds, List<IFormFile> contentFiles, UserDTO user);
    }
}
