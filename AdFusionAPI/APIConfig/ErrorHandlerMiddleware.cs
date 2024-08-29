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
            var exceptionMessage = "Lỗi hệ thống. Vui lòng liên hệ với bộ phận hỗ trợ nếu vấn đề vẫn tiếp diễn.";

            if (exception is UnauthorizedAccessException)
            {
                statusCode = HttpStatusCode.Unauthorized; // 401 Unauthorized
                exceptionMessage = "Bạn cần phải đăng nhập để truy cập tài nguyên này.";
            }
            else if (exception is InvalidOperationException)
            {
                statusCode = HttpStatusCode.BadRequest; // 400 Bad Request
                exceptionMessage = exception.Message;
            }
            else if (exception is AccessViolationException)
            {
                statusCode = HttpStatusCode.Forbidden; // 403 Forbidden
                exceptionMessage = "Tài khoản của bạn không có quyền truy cập.";
            }
            else if (exception is KeyNotFoundException)
            {
                statusCode = HttpStatusCode.NotFound; // 404 Not Found
                exceptionMessage = "Tài nguyên bạn yêu cầu không tồn tại.";
            }

            context.Response.StatusCode = (int)statusCode;

            var errorResponse = new
            {
                message =  exceptionMessage,
                statusCode = context.Response.StatusCode
            };

            _logger.Error("System has error: " + exception.ToString());
            return context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }
    }
}
