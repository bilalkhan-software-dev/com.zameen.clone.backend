namespace com.zameen.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = context.TraceIdentifier;

        context.Response.Headers.TryAdd("X-Trace-Id", traceId);

        using (Serilog.Context.LogContext.PushProperty("TraceId", traceId))
        {
            await _next(context);
        }
    }
}
