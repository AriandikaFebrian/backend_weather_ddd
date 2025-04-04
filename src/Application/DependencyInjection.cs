// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using NetCa.Application.Common.Behaviors;

namespace NetCa.Application;

/// <summary>
/// DependencyInjection
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// AddApplicationServices
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddAutoMapper(assembly);
        services.AddValidatorsFromAssembly(assembly);
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenRequestPreProcessor(typeof(LoggingBehavior<>));
            cfg.AddOpenBehavior(typeof(RateLimitPolicyBehavior<,>), ServiceLifetime.Singleton);
            cfg.AddOpenBehavior(typeof(BulkheadPolicyBehavior<,>), ServiceLifetime.Singleton);
            cfg.AddOpenBehavior(typeof(CircuitBreakerPolicyBehavior<,>), ServiceLifetime.Singleton);
            cfg.AddOpenBehavior(typeof(TimeoutPolicyBehavior<,>));
            cfg.AddOpenBehavior(typeof(UnhandledExceptionBehavior<,>));
            cfg.AddOpenBehavior(typeof(RequestBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(PerformanceBehavior<,>));
            cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
            cfg.AddOpenBehavior(typeof(FallbackBehavior<,>));
            cfg.AddOpenBehavior(typeof(RetryPolicyBehavior<,>));
        });

        return services;
    }
}
