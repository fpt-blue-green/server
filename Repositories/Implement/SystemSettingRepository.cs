using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Helper;
using Repositories.Interface;

namespace Repositories.Implement
{
    public class SystemSettingRepository : SingletonBase<SystemSettingRepository>, ISystemSettingRepository
    {
        public SystemSettingRepository() { }


        public async Task<SystemSetting> GetSystemSetting(string keyName)
        {
            var systemSetting = await context.SystemSettings.FirstOrDefaultAsync(s => s.KeyName == keyName);
            return systemSetting!;
        }

        public async Task UpdateSystemSetingKeyValue(SystemSetting systemSetting)
        {
            context.Entry<SystemSetting>(systemSetting).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}
