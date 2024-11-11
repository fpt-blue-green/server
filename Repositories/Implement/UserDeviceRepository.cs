using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;

namespace Repositories.Implement
{
    public class UserDeviceRepository : IUserDeviceRepository
    {
        public async Task<IEnumerable<UserDevice>> GetAll()
        {
            using (var context = new PostgresContext())
            {
                var userDevices = await context.UserDevices.ToListAsync();
                return userDevices!;
            }
        }

        public async Task<IEnumerable<UserDevice>> GetAllIgnoreFilter()
        {
            using (var context = new PostgresContext())
            {
                var userDevices = await context.UserDevices.IgnoreQueryFilters().ToListAsync();
                return userDevices!;
            }
        }

        public async Task<IEnumerable<UserDevice>> GetByUserId(Guid userId)
        {
            using (var context = new PostgresContext())
            {
                var userDevices = await context.UserDevices.Where(u => u.UserId == userId).OrderByDescending(u => u.LastLoginTime).ToListAsync();
                return userDevices!;
            }
        }

        public async Task<UserDevice> GetUserDeviceByAgentAndToken(BrowserInfo userAgentConverted, string refreshToken)
        {
            using (var context = new PostgresContext())
            {
                var userDevices = await context.UserDevices.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken
                                                            && u.User.IsBanned == false
                                                            && u.DeviceOperatingSystem == userAgentConverted.DeviceOperatingSystem
                                                            && u.BrowserName == userAgentConverted.BrowserName
                                                            && u.DeviceType == userAgentConverted.DeviceType);
                return userDevices!;
            }
        }

        public async Task<UserDevice> GetUserDeviceByAgentAndUserID(BrowserInfo userAgentConverted, Guid userId)
        {
            using (var context = new PostgresContext())
            {
                var userDevices = await context.UserDevices.FirstOrDefaultAsync(u => u.UserId == userId 
                                                            && u.User.IsBanned == false
                                                            && u.DeviceOperatingSystem == userAgentConverted.DeviceOperatingSystem
                                                            && u.BrowserName == userAgentConverted.BrowserName
                                                            && u.DeviceType == userAgentConverted.DeviceType);
                return userDevices!;
            }
        }

        public async Task Update(UserDevice userDevice)
        {
            using (var context = new PostgresContext())
            {
                context.Entry(userDevice).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateRange(List<UserDevice> userDevices)
        {
            using (var context = new PostgresContext())
            {
                foreach (var userDevice in userDevices)
                {
                    context.Entry(userDevice).State = EntityState.Modified;
                }

                await context.SaveChangesAsync();
            }
        }


        public async Task Create(UserDevice userDevice)
        {
            using (var context = new PostgresContext())
            {
                context.UserDevices.Add(userDevice);
                await context.SaveChangesAsync();
            }
        }
    }
}
