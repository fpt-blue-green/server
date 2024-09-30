using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;

namespace Repositories.Implement
{
    public class UserDeviceRepository : IUserDeviceRepository
    {

        public async Task<IEnumerable<UserDevice>> GetByUserId(Guid userId)
        {
            using (var context = new PostgresContext())
            {
                var userDevices = await context.UserDevices.Where(u => u.UserId == userId).ToListAsync();
                return userDevices!;
            }
        }
        public async Task<UserDevice> GetUserDeviceByAgentAndToken(BrowserInfo userAgentConverted, string refreshToken)
        {
            using (var context = new PostgresContext())
            {
                var userDevices = await context.UserDevices.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken
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
