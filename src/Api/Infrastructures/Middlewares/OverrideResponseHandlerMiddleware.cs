// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Interfaces;
using NetCa.Application.Common.Models;
using NetCa.Domain.Constants;
using Newtonsoft.Json.Linq;

namespace NetCa.Api.Infrastructures.Middlewares;

/// <summary>
/// OverrideResponseHandlerMiddleware
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="OverrideResponseHandlerMiddleware"/> class.
/// </remarks>
/// <param name="_next"></param>
/// <param name="_redisService"></param>
/// <param name="_appSetting"></param>
/// <param name="_logger"></param>
/// <returns></returns>
public class OverrideResponseHandlerMiddleware(
    RequestDelegate _next,
    IRedisService _redisService,
    AppSetting _appSetting,
    ILogger<OverrideResponseHandlerMiddleware> _logger)
{
    /// <summary>
    /// InvokeAsync
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await _next(context).ConfigureAwait(false);
            return;
        }

        _logger.LogDebug("Overriding Response");

        var watch = new Stopwatch();
        watch.Start();

        var originalBody = context.Response.Body;

        try
        {
            var statusCode = 0;
            var responseBody = string.Empty;
            var policyName = string.Empty;

            var memStream = new MemoryStream();

            await using (memStream.ConfigureAwait(false))
            {
                context.Response.Body = memStream;

                await _next(context).ConfigureAwait(false);

                policyName = (string)context.Items["policy"];
                statusCode = context.Response.StatusCode;
                memStream.Position = 0;

                if (
                    statusCode != 200
                    || !context.Response.ContentType.Contains(HeaderConstants.Json)
                )
                {
                    await memStream.CopyToAsync(originalBody).ConfigureAwait(false);
                    return;
                }

                responseBody = await new StreamReader(memStream)
                    .ReadToEndAsync()
                    .ConfigureAwait(false);

                watch.Stop();
                var responseTimeForCompleteRequest = watch.ElapsedMilliseconds;

                context = await RedisCachingAsync(policyName, context, responseBody)
                    .ConfigureAwait(false);

                var buffer = Encoding
                    .UTF8
                    .GetBytes(ToJsonApi(statusCode, responseTimeForCompleteRequest, responseBody));

                context.Response.ContentLength = buffer.Length;

                var output = new MemoryStream(buffer);
                await using (output.ConfigureAwait(false))
                {
                    output.Position = 0;
                    await output.CopyToAsync(originalBody).ConfigureAwait(false);
                }
            }
        }
        catch
        {
            watch.Stop();

            throw;
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }

    private static string ToJsonApi(
        int statusCode,
        long responseTimeForCompleteRequest,
        string responseBody)
    {
        var json = JObject.Parse(responseBody);
        json["responseTime"] = responseTimeForCompleteRequest;
        if (json["status"] is not JObject status)
        {
            var stat = new JObject
            {
                { "code", statusCode },
                { "desc", ReasonPhrases.GetReasonPhrase(statusCode) }
            };
            json.Add("status", stat);
        }
        else
        {
            status["code"] = statusCode;
            status["desc"] = ReasonPhrases.GetReasonPhrase(statusCode);
        }

        return json.ToString();
    }

    private async Task<HttpContext> RedisCachingAsync(
        string policyName,
        HttpContext context,
        string responseBody)
    {
        var requestIfNoneMatch =
            context.Request.Headers[HeaderConstants.IfNoneMatch].ToString() ?? "";
        if (string.IsNullOrEmpty(requestIfNoneMatch))
        {
            return context;
        }

        var policy = IsCache(policyName);
        if (policy is not { IsCache: true })
        {
            return context;
        }

        var key = await _redisService
            .SaveSubAsync(RedisConstants.SubKeyHttpRequest, policy.Name, responseBody)
            .ConfigureAwait(false);
        context.Response.Headers[HeaderConstants.ETag] = key;
        return context;
    }

    private Policy IsCache(string policy)
    {
        var policyList = _appSetting.Redis.Policy;
        return policyList.Count.Equals(0)
            ? null
            : policyList.SingleOrDefault(x => x.Name.ToLower().Equals(policy.ToLower()));
    }
}

/// <summary>
/// OverrideResponseMiddlewareExtensions
/// </summary>
public static class OverrideResponseMiddlewareExtensions
{
    /// <summary>
    /// UseOverrideResponseHandler
    /// </summary>
    /// <param name="builder"></param>
    public static void UseOverrideResponseHandler(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<OverrideResponseHandlerMiddleware>();
    }
}
