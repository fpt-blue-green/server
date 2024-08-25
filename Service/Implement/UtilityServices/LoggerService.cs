using Microsoft.Extensions.Configuration;
using Serilog;

public class LoggerService
{
    private static ILogger _consoleLogger;
    private static ILogger _Logger;
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

        // Logger cho console
        _consoleLogger = new LoggerConfiguration()
            .WriteTo.Console() // Log ra console
            .CreateLogger();

        // Logger cho cơ sở dữ liệu
        _Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        // Đăng ký sự kiện ứng dụng kết thúc để dọn dẹp logger
        AppDomain.CurrentDomain.ProcessExit += (sender, args) => CloseAndFlush();
    }

    public ILogger GetConsoleLogger()
    {
        return _consoleLogger;
    }

    public ILogger GetDbLogger()
    {
        return _Logger;
    }

    public void CloseAndFlush()
    {
        if (_consoleLogger != null)
        {
            Log.CloseAndFlush();
            _consoleLogger = null;
        }

        if (_Logger != null)
        {
            Log.CloseAndFlush();
            _Logger = null;
        }
    }
}
