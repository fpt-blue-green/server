using System.Diagnostics;
using ILogger = Serilog.ILogger;

namespace AdFusionAPI.APIConfig
{
    public class RequestLogMiddleware
    {
        private readonly RequestDelegate _next;
        private static ILogger _loggerService = new LoggerService().GetConsoleLogger();


        public RequestLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = Stopwatch.GetTimestamp();

            // Lưu trữ mã trạng thái để ghi log sau khi request được xử lý
            int statusCode = 0;

            // Ghi lại mã trạng thái trước khi tiếp tục xử lý request
            context.Response.OnStarting(() =>
            {
                statusCode = context.Response.StatusCode;
                return Task.CompletedTask;
            });

            await _next(context);

            var endTime = Stopwatch.GetTimestamp();
            var duration = (endTime - startTime) / (double)Stopwatch.Frequency * 1000; // Convert to milliseconds

            // Ghi log
            _loggerService.Information("HTTP {Method} {Path} responded {StatusCode} in {Duration} ms",
                context.Request.Method,
                context.Request.Path,
                statusCode,
                duration);
        }
    }
}
