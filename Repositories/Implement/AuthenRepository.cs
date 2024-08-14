﻿using BusinessObjects.Models;
using BusinessObjects.ModelsDTO.AuthenDTO;
using Microsoft.EntityFrameworkCore;
using Repositories.Helper;
using Repositories.Interface;

namespace Repositories.Implement
{
    public class AuthenRepository : SingletonBase<AuthenRepository>, IAuthenRepository
    {
        public AuthenRepository() { }
        public async Task<List<User>> GetUsers()
        {
            try
            {
                var users = await context.Users.ToListAsync();
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> GetUserById(Guid userId)
        {
            try
            {
                var user = await context.Users.SingleOrDefaultAsync(u => u.Id == userId);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> GetUserByEmail(string email)
        {
            try
            {
                var user = await context.Users.SingleOrDefaultAsync(u => u.Email == email);
                    return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> GetUserByLoginDTO(LoginDTO loginDTO)
        {
            try
            {
                var user = await context.Users
                                    .Include(u => u.BannedUserUsers).Include(u => u.Influencers)
                                    .Where(u => u.Email == loginDTO.Email && u.Password == loginDTO.Password)
                                    .FirstOrDefaultAsync();
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateUser(User user)
        {
            try
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task UpdateUser(User user)
        {
            try
            {
                context.Entry<User>(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
