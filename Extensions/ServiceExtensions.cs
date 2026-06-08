using System.Security.Claims;
using System.Text;
using com.zameen.Data;
using com.zameen.Exceptions;
using com.zameen.Models;
using com.zameen.Repositories.Implementation;
using com.zameen.Repositories.Interfaces;
using com.zameen.Services;
using com.zameen.Services.Implementation;
using com.zameen.Services.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace com.zameen.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    // This makes ALL enums appear as their name (e.g., "PENDING", "Approved")
                    options.JsonSerializerOptions.Converters.Add(
                        new System.Text.Json.Serialization.JsonStringEnumConverter()
                    );
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    // Intercept automatic 400 responses and throw our custom exception
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        List<string> errors = context
                            .ModelState.Where(e => e.Value!.Errors.Count > 0)
                            .SelectMany(e => e.Value!.Errors.Select(er => er.ErrorMessage))
                            .ToList();
                        throw new AppValidationException(errors);
                    };
                });

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    sqlOptions =>
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null
                        )
                )
            );

            // Identity
            services
                .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Authentication: JWT + Google
            var jwtKey = configuration["Jwt:Key"]!;
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        NameClaimType = ClaimTypes.NameIdentifier,
                        RoleClaimType = ClaimTypes.Role,
                        ClockSkew = TimeSpan.Zero,
                    };
                })
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = configuration["Authentication:Google:ClientId"]!;
                    googleOptions.ClientSecret = configuration[
                        "Authentication:Google:ClientSecret"
                    ]!;
                    googleOptions.SignInScheme = IdentityConstants.ExternalScheme;
                });

            // Authorization policies
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("AgentOnly", policy => policy.RequireRole("Agent"));
                options.AddPolicy(
                    "AdminAndAgentOnly",
                    policy =>
                    {
                        policy.RequireRole("Agent");
                        policy.RequireRole("Admin");
                    }
                );
            });

            // FluentValidation
            services.AddFluentValidationAutoValidation(config =>
            {
                config.DisableDataAnnotationsValidation = true;
            });
            services.AddValidatorsFromAssemblyContaining<Program>();

            // Application Repositories
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IAgentRepository, AgentRepository>();
            services.AddScoped<IPropertyRepository, PropertyRepository>();
            services.AddScoped<IEnquiryRepository, EnquiryRepository>();
            services.AddScoped<ISearchLogRepository, SearchLogRepository>();
            services.AddScoped<IPriceTrendRepository, PriceTrendRepository>();

            services.AddMemoryCache();

            // Application services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAgentService, AgentService>();
            services.AddScoped<IPropertyService, PropertyService>();
            services.AddScoped<IEnquiryService, EnquiryService>();
            services.AddScoped<ITrendingService, TrendingService>();
            services.AddScoped<IPriceTrendService, PriceTrendService>();

            services.AddScoped<JwtTokenService>();

            services.AddHostedService<PriceTrendGenerationService>();
            // AutoMapper
            services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
            });

            var frontendUrl = configuration["Frontend:BaseUrl"];

            if (string.IsNullOrEmpty(frontendUrl))
            {
                // Fallback
                frontendUrl = "http://localhost:3000";
            }

            services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowNextJsFrontend",
                    builder =>
                    {
                        builder
                            .WithOrigins(frontendUrl) // Restricts incoming browser requests
                            .AllowAnyMethod() // Allows GET, POST, PUT, DELETE, etc.
                            .AllowAnyHeader() // Allows Authorization, Content-Type headers
                            .AllowCredentials(); // Crucial if your Next.js app sends HttpOnly Refresh Cookies
                    }
                );
            });
            // Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PropertyHub API", Version = "v1" });

                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                    }
                );
            });

            // Health checks
            services.AddHealthChecks();

            return services;
        }
    }
}
