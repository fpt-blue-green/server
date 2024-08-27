using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Http;

namespace Service.Interface
{
    public interface IUserService
    {
        Task<ApiResponse<string>> UploadImageAsync(IFormFile file, string token);
    }
}
