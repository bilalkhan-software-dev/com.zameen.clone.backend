using com.zameen.Data;
using com.zameen.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog(
    (context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext();
    }
);

// Register all services from the extension method
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure middleware
app.UseApplicationMiddleware(app.Environment);

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.SeedRolesAndAdminAsync(services);
}

app.Run();
