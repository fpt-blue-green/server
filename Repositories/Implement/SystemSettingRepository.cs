using System.Threading.Tasks;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class SystemSettingRepository : ISystemSettingRepository
    {
        public async Task<IEnumerable<SystemSetting>> GetAll()
        {
            using (var context = new PostgresContext())
            {
                var systemSetting = await context.SystemSettings.ToListAsync();
                return systemSetting!;
            }
        }

        public async Task<SystemSetting> GetSystemSetting(string keyName)
        {
            using (var context = new PostgresContext())
            {
                var systemSetting = await context.SystemSettings
                    .FirstOrDefaultAsync(s => s.KeyName == keyName);
                return systemSetting!;
            }
        }

        public async Task UpdateSystemSettingKeyValue(SystemSetting systemSetting)
        {
            using (var context = new PostgresContext())
            {
                context.Entry<SystemSetting>(systemSetting).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
    }
}
