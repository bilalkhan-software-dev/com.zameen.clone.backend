using System.Diagnostics;

namespace com.zameen.Middleware;

public sealed class RequestResponseLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestResponseLoggingMiddleware> logger
)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        var traceId = context.TraceIdentifier;

        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["TraceId"] = traceId }
        );

        try
        {
            _logger.LogInformation(
                "Request Started | {Method} {Path} | TraceId={TraceId}",
                context.Request.Method,
                context.Request.Path,
                traceId
            );

            await _next(context);
        }
        catch
        {
            _logger.LogError(
                "Request Failed | {Method} {Path} | TraceId={TraceId}",
                context.Request.Method,
                context.Request.Path,
                traceId
            );

            throw;
        }
        finally
        {
            stopwatch.Stop();

            _logger.LogInformation(
                "Request Completed | {Method} {Path} | StatusCode={StatusCode} | Duration={Duration}ms | TraceId={TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                traceId
            );
        }
    }
}
