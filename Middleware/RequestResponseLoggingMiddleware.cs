using System.Diagnostics;

namespace com.zameen.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestResponseLoggingMiddleware> logger
        )
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log Request
            _logger.LogInformation(
                "HTTP {Method} {Path} started",
                context.Request.Method,
                context.Request.Path
            );
            var stopwatch = Stopwatch.StartNew();

            await _next(context);

            stopwatch.Stop();
            _logger.LogInformation(
                "HTTP {Method} {Path} responded {StatusCode} in {Elapsed}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds
            );
        }
    }
}
