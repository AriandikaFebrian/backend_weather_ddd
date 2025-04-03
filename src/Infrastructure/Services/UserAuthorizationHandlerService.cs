// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Models;
using NetCa.Domain.Constants;

namespace NetCa.Infrastructure.Services;

/// <summary>
/// UserAuthorizationHandlerService
/// </summary>
public class UserAuthorizationHandlerService : AuthorizationHandler<Permission>
{
    private readonly ILogger<UserAuthorizationHandlerService> _logger;

    private static readonly HttpClient _httpClient =
        new(
            new HttpHandler(new HttpClientHandler())
            {
                UsingCircuitBreaker = true,
                UsingWaitRetry = true,
                RetryCount = 4,
                SleepDuration = 1000
            }
        );

    private static readonly SemaphoreSlim SemaphoreSlim = new(1);

    /// <summary>
    /// Initializes a new instance of the <see cref="UserAuthorizationHandlerService"/> class.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="appSetting"></param>
    public UserAuthorizationHandlerService(
        ILogger<UserAuthorizationHandlerService> logger,
        AppSetting appSetting)
    {
        _logger = logger;

        _httpClient.BaseAddress ??= new Uri(appSetting.AuthorizationServer.Address);

        SemaphoreSlim.Wait();

        try
        {
            if (!_httpClient.DefaultRequestHeaders.Contains(appSetting.AuthorizationServer.Header))
            {
                _httpClient
                    .DefaultRequestHeaders
                    .Add(
                        appSetting.AuthorizationServer.Header,
                        appSetting.AuthorizationServer.Secret);
            }
        }
        catch
        {
            logger.LogDebug("Error when add request header");
        }

        SemaphoreSlim.Release();
    }

    /// <summary>
    /// HandleRequirementAsync
    /// </summary>
    /// <param name="context"></param>
    /// <param name="requirement"></param>
    /// <returns></returns>
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        Permission requirement)
    {
        var user = context.User;
        var userId = user.Claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier)?.Value;
        var clientId = user.Claims
            .FirstOrDefault(i => i.Type == UserAttributeConstants.ClientId)
            ?.Value;

        var url = $"api/authorize/{userId}/{clientId}/{requirement.Name}";

        var response = await _httpClient
            .GetAsync(new Uri(_httpClient.BaseAddress + url))
            .ConfigureAwait(false);

        _logger.LogDebug("Response: \n {response}", response.ToString());

        if (response.IsSuccessStatusCode)
        {
            context.Succeed(requirement);
        }
    }
}
