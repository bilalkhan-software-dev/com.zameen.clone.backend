using com.zameen.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog(
    (ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration)
);

// Register all services from the extension method
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure middleware
app.UseApplicationMiddleware(app.Environment);

// Optional: seed admin/roles on startup
// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     await SeedData.SeedRolesAndAdminAsync(services);
// }

app.Run();
