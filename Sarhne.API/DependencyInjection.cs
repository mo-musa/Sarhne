using FluentValidation;
using Hangfire;
using Hangfire.SqlServer;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sarhne.API.Authorization;
using Sarhne.API.Hubs;
using Sarhne.API.Middlewares;
using Sarhne.Application;
using Sarhne.Application.Behaviors;
using Sarhne.Application.Common.Mappings;
using Sarhne.Application.Common.Settings;
using Sarhne.Application.Contracts.Persistence;
using Sarhne.Application.Contracts.Services.Authentication.Cookies;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using Sarhne.Application.Contracts.Services.Authentication.Token;
using Sarhne.Application.Contracts.Services.BackgroundJobs;
using Sarhne.Application.Contracts.Services.Caching;
using Sarhne.Application.Contracts.Services.Email;
using Sarhne.Application.Contracts.Services.Messages;
using Sarhne.Application.Contracts.Services.Notifications;
using Sarhne.Application.Contracts.Services.Storage;
using Sarhne.Domain.Constants;
using Sarhne.Domain.Entities.Identity;
using Sarhne.Infrastructure.Identity;
using Sarhne.Infrastructure.Persistence;
using Sarhne.Infrastructure.Persistence.Interceptors;
using Sarhne.Infrastructure.Services.Authentication.Cookies;
using Sarhne.Infrastructure.Services.Authentication.CurrentUser;
using Sarhne.Infrastructure.Services.Authentication.Token;
using Sarhne.Infrastructure.Services.BackgroundJobs;
using Sarhne.Infrastructure.Services.Caching;
using Sarhne.Infrastructure.Services.Email;
using Sarhne.Infrastructure.Services.Notifications;
using Sarhne.Infrastructure.Services.Storage;
using Sarhne.Infrastructure.Settings;
using System.Text;
using System.Text.Json.Serialization;
namespace Sarhne.API;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddPersistence(configuration)
            .AddIdentityServices(configuration)
            .AddApplicationServices()
            .AddOptions(configuration)
            .AddAuthenticationServices(configuration)
            .AddInfrastructureServices()
            .AddSignalRServices()
            .AddHangfireServices(configuration)
            .AddCaching()
            .AddMapping()
            .AddApiServices()
            .AddCorsServices(configuration)
            .AddRateLimiting();

        return services;
    }

    // ---------------------------------------------------------
    // Persistence
    // ---------------------------------------------------------

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<AuditableSoftDeleteInterceptor>();

        services.AddDbContext<SarhneDbContext>(
            (serviceProvider, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString(
                        "DefaultConnection"));

                options.AddInterceptors(
                    serviceProvider.GetRequiredService<
                        AuditableSoftDeleteInterceptor>());
            });

        services.AddScoped<ISarhneDbContext>(
            provider =>
                provider.GetRequiredService<SarhneDbContext>());

        return services;
    }

    // ---------------------------------------------------------
    // Identity
    // ---------------------------------------------------------

    private static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddIdentity<ApplicationUser, ApplicationRole>(
                options =>
                {
                    options.User.RequireUniqueEmail = true;

                    options.Password.RequiredLength = 8;
                    options.Password.RequireDigit = true;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                })
            .AddEntityFrameworkStores<SarhneDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<SeedAdminSettings>(
            configuration.GetSection(
                SeedAdminSettings.SectionName));

        services.AddScoped<IdentitySeeder>();

        return services;
    }

    // ---------------------------------------------------------
    // Application
    // ---------------------------------------------------------

    private static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(AssemblyReference).Assembly);

            cfg.AddOpenBehavior(
                typeof(LoggingBehavior<,>));

            cfg.AddOpenBehavior(
                typeof(PerformanceBehavior<,>));

            cfg.AddOpenBehavior(
                typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(
            typeof(AssemblyReference).Assembly);

        return services;
    }

    // ---------------------------------------------------------
    // Options
    // ---------------------------------------------------------

    private static IServiceCollection AddOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(
                JwtSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.Configure<CookieSettings>(
            configuration.GetSection(
                CookieSettings.SectionName));

        services
            .AddOptions<EmailSettings>()
            .Bind(configuration.GetSection(
                EmailSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<ApplicationSettings>()
            .Bind(configuration.GetSection(
                ApplicationSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    // ---------------------------------------------------------
    // Authentication & Authorization
    // ---------------------------------------------------------

    private static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration
            .GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>()
            ?? throw new InvalidOperationException(
                "JWT settings are not configured.");

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    jwtSettings.SecretKey)),

                        ValidateLifetime = true,

                        ClockSkew = TimeSpan.Zero
                    };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken =
                            context.Request.Query["access_token"];

                        var path =
                            context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                Policies.Admin,
                policy =>
                {
                    policy.RequireRole(
                        Roles.Admin,
                        Roles.SuperAdmin);
                });

            options.AddPolicy(
                Policies.SuperAdmin,
                policy =>
                {
                    policy.RequireRole(
                        Roles.SuperAdmin);
                });
        });

        return services;
    }
    // ---------------------------------------------------------
    // Application Services
    // ---------------------------------------------------------

    private static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ICookieService, CookieService>();

        services.AddHttpContextAccessor();

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<
            IEmailTemplateRenderer,
            EmailTemplateRenderer>();

        services.AddScoped<
            IFileStorageService,
            LocalFileStorageService>();

        services.AddScoped<
            INotificationService,
            NotificationService>();

        services.AddScoped<
            INotificationRealtimeService,
            NotificationRealtimeService>();

        services.AddScoped<
            IMessageRealtimeService,
            MessageRealtimeService>();

        services.AddScoped<ICleanupJob, CleanupJob>();

        return services;
    }

    // ---------------------------------------------------------
    // SignalR
    // ---------------------------------------------------------

    private static IServiceCollection AddSignalRServices(
        this IServiceCollection services)
    {
        services
            .AddSignalR()
            .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions
                    .Converters.Add(
                        new JsonStringEnumConverter());
            });

        services.AddSingleton<IUserIdProvider, SarhneUserIdProvider>();

        return services;
    }

    // ---------------------------------------------------------
    // Hangfire
    // ---------------------------------------------------------

    private static IServiceCollection AddHangfireServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHangfire(config =>
        {
            config
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(
                    configuration.GetConnectionString(
                        "DefaultConnection"),
                    new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout =
                            TimeSpan.FromMinutes(5),

                        SlidingInvisibilityTimeout =
                            TimeSpan.FromMinutes(5),

                        QueuePollInterval =
                            TimeSpan.Zero,

                        UseRecommendedIsolationLevel =
                            true,

                        DisableGlobalLocks =
                            true
                    });
        });

        services.AddHangfireServer();

        return services;
    }

    // ---------------------------------------------------------
    // Caching
    // ---------------------------------------------------------

    private static IServiceCollection AddCaching(
        this IServiceCollection services)
    {
        services.AddMemoryCache();

        services.AddSingleton<
            ICacheService,
            MemoryCacheService>();

        return services;
    }

    // ---------------------------------------------------------
    // Mapping
    // ---------------------------------------------------------

    private static IServiceCollection AddMapping(
        this IServiceCollection services)
    {
        var mapsterConfig =
            TypeAdapterConfig.GlobalSettings;

        mapsterConfig.Scan(
            typeof(UserMappingConfig).Assembly);

        services.AddSingleton(mapsterConfig);

        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }

    // ---------------------------------------------------------
    // API
    // ---------------------------------------------------------

    private static IServiceCollection AddApiServices(
        this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions
                    .Converters.Add(
                        new JsonStringEnumConverter()); // to use enum as string
            });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddProblemDetails();

        services.AddExceptionHandler<
            GlobalExceptionHandler>();

        services.AddHealthChecks()
            .AddDbContextCheck<SarhneDbContext>();

        return services;
    }

    // ---------------------------------------------------------
    // Cors
    // ---------------------------------------------------------
    private static IServiceCollection AddCorsServices(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        var allowedOrigins =
            configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy(
                "Frontend",
                policy =>
                {
                    policy
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });

        return services;
    }

    // ---------------------------------------------------------
    // Rate limit
    // ---------------------------------------------------------
    private static IServiceCollection AddRateLimiting(
    this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddFixedWindowLimiter(
                "fixed",
                limiterOptions =>
                {
                    limiterOptions.PermitLimit = 5;
                    limiterOptions.Window = TimeSpan.FromMinutes(1);
                    limiterOptions.QueueLimit = 0;
                });
        });

        return services;
    }
}
