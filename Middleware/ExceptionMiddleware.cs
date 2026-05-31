using System.Text.Json;
using com.zameen.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace com.zameen.Middleware;

public sealed class ExceptionMiddleware(
    RequestDelegate next,
    ILogger<ExceptionMiddleware> logger,
    IHostEnvironment environment
)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;
    private readonly IHostEnvironment _environment = environment;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
                "Request was cancelled. | TraceId={TraceId} {Method} {Path}",
                context.TraceIdentifier,
                context.Request.Method,
                context.Request.Path
            );
        }
        catch (AppException ex)
        {
            _logger.LogWarning(
                ex,
                "Application Exception | TraceId={TraceId} | {Method} {Path} | ErrorCode: {ErrorCode}",
                context.TraceIdentifier,
                context.Request.Method,
                context.Request.Path,
                ex.ErrorCode
            );

            await HandleAppExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled Exception | TraceId={TraceId} | {Method} {Path}",
                context.TraceIdentifier,
                context.Request.Method,
                context.Request.Path
            );

            _logger.LogError(
                ex,
                "Unhandled Exception | TraceId={TraceId} | Path={Path}",
                context.TraceIdentifier,
                context.Request.Path
            );
            await HandleUnhandledExceptionAsync(context, ex, _environment);
        }
    }

    private static async Task HandleAppExceptionAsync(HttpContext context, AppException exception)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.Clear();
        context.Response.StatusCode = exception.HttpStatusCode;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = exception.HttpStatusCode,
            Title = exception.Message,
            Instance = context.Request.Path,
        };

        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        if (exception is AppValidationException validationEx)
        {
            problemDetails.Extensions["errors"] = validationEx.Errors;
        }

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }

    private static async Task HandleUnhandledExceptionAsync(
        HttpContext context,
        Exception exception,
        IHostEnvironment environment
    )
    {
        if (context.Response.HasStarted)
            return;

        context.Response.Clear();
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Detail = environment.IsDevelopment()
                ? exception.ToString()
                : "An unexpected error occurred.",
            Instance = context.Request.Path,
        };

        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }
}
