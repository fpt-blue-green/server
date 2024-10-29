using System.Diagnostics;
using System.Text;
using ILogger = Serilog.ILogger;

namespace AdFusionAPI.APIConfig
{
    public class RequestLogMiddleware
    {
        private readonly RequestDelegate _next;
        private static ILogger _loggerService = new LoggerService().GetConsoleLogger();
        private static ILogger _dbLoggerService = new LoggerService().GetDbLogger();


        public RequestLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var startTime = Stopwatch.GetTimestamp();

            // Đọc request body
            context.Request.EnableBuffering();
            string requestBody = await ReadRequestBodyAsync(context.Request);

            await _next(context);  // Xử lý request

            var endTime = Stopwatch.GetTimestamp();
            var duration = (endTime - startTime) / (double)Stopwatch.Frequency * 1000;

            // Lấy mã trạng thái sau khi request đã được xử lý
            var statusCode = context.Response.StatusCode;

            // Ghi log ra console
            _loggerService.Information("HTTP {Method} {Path} {QueryString} responded {StatusCode} in {Duration} ms",
                context.Request.Method, context.Request.Path, context.Request.QueryString, statusCode, duration.ToString("F4"));

            // Ghi log ra database
            _dbLoggerService.Information("HTTP {Method} {Path} {QueryString} responded {StatusCode} in {Duration} ms. Request Body: {Body}",
                context.Request.Method, context.Request.Path, context.Request.QueryString, statusCode, duration.ToString("F4"), requestBody);
        }

        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            string body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }

    }
}
