
using BusinessObjects;
using Microsoft.AspNetCore.Http;

namespace Service
{
    public interface IUserService
    {
        Task<string> UploadAvatarAsync(IFormFile file, string folder, UserDTO user);
    }
}
