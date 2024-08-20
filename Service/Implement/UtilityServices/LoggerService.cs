using Microsoft.Extensions.Configuration;
using Serilog;

public class LoggerService
{
    private static ILogger _logger;
    private static bool isConfigured = false;

    public LoggerService()
    {
        if (!isConfigured)
        {
            ConfigureLogger();
            isConfigured = true;
        }
    }

    private void ConfigureLogger()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("Resources/appsettings.json")
            .Build();

        _logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        // Đăng ký sự kiện ứng dụng kết thúc để dọn dẹp logger
        AppDomain.CurrentDomain.ProcessExit += (sender, args) => CloseAndFlush();
    }

    public ILogger GetLogger()
    {
        return _logger;
    }

    public void CloseAndFlush()
    {
        if (_logger != null)
        {
            Log.CloseAndFlush();
            _logger = null;
        }
    }
}
