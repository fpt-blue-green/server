
using BusinessObjects;
using Microsoft.AspNetCore.Http;

namespace Service
{
    public interface IUserService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder, UserDTO user, string fileSuffix);
        Task<List<string>> UploadContentImages(List<string> imageList, List<IFormFile> contentFiles, UserDTO user);
    }
}
