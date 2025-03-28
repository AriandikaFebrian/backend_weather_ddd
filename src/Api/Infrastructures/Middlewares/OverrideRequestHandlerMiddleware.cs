// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Interfaces;
using NetCa.Domain.Constants;

namespace NetCa.Api.Infrastructures.Middlewares;

/// <summary>
/// OverrideRequestHandlerMiddleware
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="OverrideRequestHandlerMiddleware"/> class.
/// </remarks>
/// <param name="_next"></param>
/// <param name="_redisService"></param>
/// <param name="_logger"></param>
/// <returns></returns>
public class OverrideRequestHandlerMiddleware(
    RequestDelegate _next,
    IRedisService _redisService,
    ILogger<OverrideRequestHandlerMiddleware> _logger)
{
    /// <summary>
    /// InvokeAsync
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogDebug("Overriding Request");

        if (context.Request.Path.StartsWithSegments("/api"))
        {
            var requestIfNoneMatch = context.Request.Headers[HeaderConstants.IfNoneMatch].ToString() ?? "";

            if (!string.IsNullOrEmpty(requestIfNoneMatch))
            {
                var encodedEntity = await _redisService.GetAsync(requestIfNoneMatch).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(encodedEntity))
                {
                    const int code = (int)HttpStatusCode.NotModified;
                    context.Response.StatusCode = code;
                    return;
                }
            }
        }

        await _next(context).ConfigureAwait(false);
    }
}

/// <summary>
/// OverrideRequestMiddlewareExtensions
/// </summary>
public static class OverrideRequestMiddlewareExtensions
{
    /// <summary>
    /// UseOverrideRequestHandler
    /// </summary>
    /// <param name="builder"></param>
    public static void UseOverrideRequestHandler(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<OverrideRequestHandlerMiddleware>();
    }
}
