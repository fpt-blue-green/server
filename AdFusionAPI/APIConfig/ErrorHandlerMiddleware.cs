using ILogger = Serilog.ILogger;
using Newtonsoft.Json;
using System.Net;

namespace AdFusionAPI.APIConfig
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private static ILogger _logger = new LoggerService().GetDbLogger();

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // Tiếp tục xử lý request
                await _next(context);
            }
            catch (Exception ex)
            {
                // Bắt lỗi và xử lý
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = HttpStatusCode.InternalServerError; // 500 nếu là lỗi không xác định
            var message = "Đã xảy ra lỗi không xác định.";

            if (exception is UnauthorizedAccessException)
            {
                statusCode = HttpStatusCode.Unauthorized; // 401 Unauthorized
                message = "Bạn không có quyền truy cập.";
            }
            else if (exception is ArgumentException)
            {
                statusCode = HttpStatusCode.BadRequest; // 400 Bad Request
                message = "Yêu cầu không hợp lệ.";
            }

            context.Response.StatusCode = (int)statusCode;

            var errorResponse = new
            {
                message = exception.Message,
                statusCode = context.Response.StatusCode
            };

            _logger.Error("Update Influencer: " + exception.ToString());
            return context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }
    }
}
