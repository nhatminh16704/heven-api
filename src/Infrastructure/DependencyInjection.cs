using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Domain.Constants;
using Heven.Api.Infrastructure.Configuration;
using Heven.Api.Infrastructure.Data;
using Heven.Api.Infrastructure.Data.Interceptors;
using Heven.Api.Infrastructure.Identity;
using Heven.Api.Infrastructure.Services;
using Heven.Api.Infrastructure.BackgroundServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using Stripe; // Add this using directive for Stripe
using Hangfire;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");


        
        // Cấu hình Stripe
        var stripeSecretKey = configuration.GetSection("StripeSettings")["SecretKey"];
        Guard.Against.NullOrWhiteSpace(stripeSecretKey, message: "Stripe SecretKey is empty or missing.");
        StripeConfiguration.ApiKey = stripeSecretKey;

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(connectionString, b => b.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        });

        var redisConnection = configuration.GetConnectionString("RedisConnection");
        Guard.Against.NullOrWhiteSpace(redisConnection, message: "RedisConnection is empty or missing.");

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
            options.InstanceName = "Heven_"; // Tiền tố cho các Key để dễ quản lý trong Redis
        });

        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnection));

        services.AddSingleton<RedLockFactory>(sp =>
        {
            var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
            return RedLockFactory.Create([new RedLockMultiplexer(multiplexer)]);
        });

        services.AddSingleton<IDistributedLockService, RedLockDistributedLockService>();

        services.AddHealthChecks()
            .AddRedis(redisConnection, name: "redis", tags: ["cache"]);

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);

        services.AddAuthorizationBuilder();

        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));

        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        services.AddTransient<IEmailSender<ApplicationUser>, MailKitEmailSender>();

        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IIdentityService, Heven.Api.Infrastructure.Identity.IdentityService>();

        services.AddAuthorization(options =>
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));

        // Đăng ký IPaymentService
        services.AddTransient<IPaymentService, StripePaymentService>();

        // Đăng ký Hangfire
        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connectionString));
            
        services.AddHangfireServer();

        // Đăng ký Booking Job Service
        services.AddScoped<IBookingJobService, BookingJobService>();
        services.AddScoped<BookingJobProcessor>();

        return services;
    }
}
