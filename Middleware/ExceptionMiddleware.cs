using System.Text.Json;
using com.zameen.Exceptions;

namespace com.zameen.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException appEx)
            {
                _logger.LogWarning(appEx, "Application exception: {ErrorCode}", appEx.ErrorCode);
                await HandleAppExceptionAsync(context, appEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleUnhandledExceptionAsync(context);
            }
        }

        // Helper methods to handle exceptions and format responses
        private static async Task HandleAppExceptionAsync(HttpContext context, AppException appEx)
        {
            context.Response.StatusCode = appEx.HttpStatusCode;
            context.Response.ContentType = "application/json";

            object response;

            if (appEx is ValidationException validationEx)
            {
                // For validation errors, include the list of errors
                response = ApiResponse<object>.Fail(appEx.Message, validationEx.Errors);
            }
            else
            {
                // All other custom exceptions
                response = ApiResponse<object>.Fail(appEx.Message);
            }

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }

        private static async Task HandleUnhandledExceptionAsync(HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            var response = ApiResponse<object>.Fail("An internal server error occurred.");
            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
