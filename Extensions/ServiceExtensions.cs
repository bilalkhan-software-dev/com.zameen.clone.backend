using System.Text;
using com.zameen.Data;
using com.zameen.Exceptions;
using com.zameen.Models;
using com.zameen.Repositories.Implementation;
using com.zameen.Repositories.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
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
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("AgentOnly", policy => policy.RequireRole("Agent"));
            });

            // FluentValidation
            services.AddFluentValidationAutoValidation(config =>
            {
                config.DisableDataAnnotationsValidation = true;
            });
            services.AddValidatorsFromAssemblyContaining<Program>();

            // Application services
            // services.AddScoped<IAuthService, AuthService>();
            // services.AddScoped<IUserService, UserService>();
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

            // AutoMapper
            services.AddAutoMapper(typeof(Program));

            // CORS (optional, for development)
            services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowAll",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
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
                        Description =
                            "JWT Authorization header using the Bearer scheme. Enter your token.",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                    }
                );

                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer",
                                },
                            },
                            Array.Empty<string>()
                        },
                    }
                );
            });

            // Health checks
            // services.AddHealthChecks().AddDbContextCheck<ApplicationDbContext>("Database");

            return services;
        }
    }
}
