// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCa.Api.Infrastructures.Handlers;
using NetCa.Application.Common.Models;
using NetCa.Infrastructure.Services;
using Serilog;

namespace NetCa.Api.Infrastructures.Middlewares;

/// <summary>
/// AuthHandlerMiddleware
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AuthenticationHandlerMiddleware"/> class.
/// </remarks>
/// <param name="_next"></param>
/// <param name="_appSetting"></param>
/// <param name="_logger"></param>
public class AuthenticationHandlerMiddleware(
    RequestDelegate _next,
    AppSetting _appSetting,
    ILogger<AuthenticationHandlerMiddleware> _logger
)
{
    /// <summary>
    /// Invoke
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
        var whitelistPathSegment =
            _appSetting.AuthorizationServer.WhiteListPathSegment?.Split(",").ToList() ?? [];
        var requiredCheck = !whitelistPathSegment.Exists(
            item => context.Request.Path.StartsWithSegments(item));

        if (requiredCheck)
        {
            _logger.LogDebug("Authenticating");

            var result = await context
                .AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme)
                .ConfigureAwait(false);

            if (!result.Succeeded)
            {
                await context.ChallengeAsync().ConfigureAwait(false);
                return;
            }
        }

        await _next(context).ConfigureAwait(false);
    }
}

/// <summary>
/// AuthHandlerMiddlewareLocal
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AuthenticationLocalHandlerMiddleware"/> class.
/// </remarks>
/// <param name="_next"></param>
public class AuthenticationLocalHandlerMiddleware(RequestDelegate _next)
{
    /// <summary>
    /// Invoke
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
        await _next(context).ConfigureAwait(false);
    }
}

/// <summary>
/// AuthHandlerMiddlewareExtensions
/// </summary>
public static class AuthenticationHandlerMiddlewareExtensions
{
    /// <summary>
    /// UseAuthHandler
    /// </summary>
    /// <param name="builder"></param>
    public static void UseAuthenticationHandler(this WebApplication builder)
    {
        Log.Information("Activating authentication.");

        builder.UseAuthentication();
        builder.UseAuthorization();

        if (builder.Environment.EnvironmentName != "Test")
        {
            builder.UseMiddleware<AuthenticationHandlerMiddleware>();
        }
        else
        {
            builder.UseMiddleware<AuthenticationLocalHandlerMiddleware>();
        }
    }

    /// <summary>
    /// AddPermissions
    /// </summary>
    /// <param name="services"></param>
    /// <param name="appSetting"></param>
    public static void AddPermissions(this IServiceCollection services, AppSetting appSetting)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = appSetting.AuthorizationServer.Address;
                options.Audience = appSetting.AuthorizationServer.Service;
                options.RequireHttpsMetadata = false;
                options.BackchannelHttpHandler = new HttpHandler(new HttpClientHandler())
                {
                    UsingCircuitBreaker = true,
                    UsingWaitRetry = true,
                    RetryCount = 4,
                    SleepDuration = 1000
                };
            });

        services.AddAuthorization(options =>
        {
            var policy = appSetting.AuthorizationServer.Policy ?? [];
            policy.ForEach(
                p => options.AddPolicy(p.Name, pol => pol.Requirements.Add(new Permission(p.Name)))
            );
        });

        services.AddSingleton<
            IAuthorizationPolicyProvider,
            UserAuthorizationPolicyProviderService
        >();
    }

    /// <summary>
    /// AddPermissions
    /// </summary>
    /// <param name="services"></param>
    /// <param name="appSetting"></param>
    public static void AddLocalPermissions(this IServiceCollection services, AppSetting appSetting)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = LocalAuthenticationHandler.AuthScheme;
                options.DefaultAuthenticateScheme = LocalAuthenticationHandler.AuthScheme;
                options.DefaultChallengeScheme = LocalAuthenticationHandler.AuthScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, LocalAuthenticationHandler>(
                LocalAuthenticationHandler.AuthScheme,
                _ => { }
            );
    }
}
