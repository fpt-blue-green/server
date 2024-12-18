﻿
using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;

namespace Service
{
    public interface IUserService
    {
        Task<string> UploadAvatarAsync(IFormFile file, string folder, UserDTO user);
        Task<FilterListResponse<UserDetailDTO>> GetAllUsers(UserFilterDTO filter);
        Task DeleteUser(Guid userId);
        Task<UserDetailDTO> GetUserById(Guid userId);
        Task<IEnumerable<UserDeviceDTO>> GetUserLoginHistory(UserDTO user);
        Task<FilterListResponse<UserPaymentDTO>> GetUserPayments(UserDTO userDTO, FilterDTO filter);
        Task<UserWalletDTO> GetUserWallet(UserDTO userDTO);
        Task<List<UserDTO>> GetUserToContact(string searchContent);
    }
}
