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

            var statusCode = HttpStatusCode.InternalServerError;
            var exceptionMessage = "Đã xảy ra lỗi hệ thống. Vui lòng liên hệ bộ phận hỗ trợ nếu vấn đề vẫn tiếp tục.";

            if (exception is UnauthorizedAccessException)
            {
                statusCode = HttpStatusCode.Unauthorized; // 401 Unauthorized
                exceptionMessage = "Bạn không có quyền truy cập. Vui lòng đăng nhập để tiếp tục.";
            }
            else if (exception is InvalidOperationException)
            {
                statusCode = HttpStatusCode.BadRequest; // 400 Bad Request
                exceptionMessage = exception.Message;
            }
            else if (exception is AccessViolationException)
            {
                statusCode = HttpStatusCode.Forbidden; // 403 Forbidden
                exceptionMessage = "Bạn không có quyền truy cập vào tài nguyên này.";
            }
            else if (exception is KeyNotFoundException)
            {
                statusCode = HttpStatusCode.NotFound; // 404 Not Found
                exceptionMessage = "Không tìm thấy tài nguyên được yêu cầu.";
            }

            context.Response.StatusCode = (int)statusCode;

            var errorResponse = new
            {
                message = exceptionMessage,
                statusCode = context.Response.StatusCode
            };

            _logger.Error("System has error: " + exception.ToString());
            return context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }
    }
}
