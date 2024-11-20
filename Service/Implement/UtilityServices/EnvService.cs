using Service.Interface.UtilityServices;

namespace Service.Implement.UtilityServices
{
    public class EnvService : IEnvService
    {
        private  bool _isLoaded = false;

        public  void LoadEnv()
        {
            if (!_isLoaded)
            {
                var basePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\Service\Resources"));
                var envPath = Path.Combine(basePath, ".env");
                // Nạp file .env
                DotNetEnv.Env.Load(envPath);

                _isLoaded = true;
            }
        }

        public  string GetEnv(string key)
        {
            LoadEnv();
            return Environment.GetEnvironmentVariable(key) ?? string.Empty;
        }
    }
}
