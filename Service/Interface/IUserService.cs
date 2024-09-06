using BusinessObjects.DTOs;
using BusinessObjects.DTOs.UserDTOs;
using Microsoft.AspNetCore.Http;

namespace Service.Interface
{
    public interface IUserService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder, UserDTO user);
        Task<List<string>> UploadContentImages(List<IFormFile> contentFiles, UserDTO user);
    }
}
