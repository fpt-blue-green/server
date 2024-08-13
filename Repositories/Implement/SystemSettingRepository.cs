using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;

namespace Repositories.Implement
{
    public class SystemSettingRepository : ISystemSettingRepository
    {
        public SystemSettingRepository() { }


        public async Task<SystemSetting> GetSystemSetting(string keyName)
        {
            var systemSetting = new SystemSetting();
            try
            {
                using (var context = new PostgresContext())
                {
                    systemSetting = await context.SystemSettings.FirstOrDefaultAsync(s => s.KeyName == keyName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return systemSetting!;
        }

        public async Task UpdateSystemSetingKeyValue(SystemSetting systemSetting)
        {
            try
            {
                using (var context = new PostgresContext())
                {
                    context.Entry<SystemSetting>(systemSetting).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
