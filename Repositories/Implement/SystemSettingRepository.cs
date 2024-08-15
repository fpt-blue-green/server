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
            try
            {
                var systemSetting = await context.SystemSettings.FirstOrDefaultAsync(s => s.KeyName == keyName);
                return systemSetting!;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateSystemSetingKeyValue(SystemSetting systemSetting)
        {
            try
            {
                context.Entry<SystemSetting>(systemSetting).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
