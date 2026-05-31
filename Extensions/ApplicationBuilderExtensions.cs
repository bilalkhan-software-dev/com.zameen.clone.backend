using System.Text.Json;
using com.zameen.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace com.zameen.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseApplicationMiddleware(
            this IApplicationBuilder app,
            IWebHostEnvironment env
        )
        {
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();

            // Swagger (only in dev)
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PropertyHub API v1")
                );
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowNextJsFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            if (app is WebApplication webApp)
            {
                webApp.MapControllers();
                webApp.MapHealthChecks(
                    "/health",
                    new HealthCheckOptions
                    {
                        ResponseWriter = async (context, report) =>
                        {
                            context.Response.ContentType = "application/json";
                            var response = new
                            {
                                status = report.Status.ToString(),
                                checks = report.Entries.Select(e => new
                                {
                                    name = e.Key,
                                    status = e.Value.Status.ToString(),
                                    description = e.Value.Description,
                                }),
                            };
                            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                        },
                    }
                );
            }
            return app;
        }
    }
}
