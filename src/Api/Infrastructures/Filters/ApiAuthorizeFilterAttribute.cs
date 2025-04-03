// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCa.Application.Common.Models;

namespace NetCa.Api.Infrastructures.Filters;

/// <summary>
/// ApiAuthorizeFilterAttribute
/// </summary>
public class ApiAuthorizeFilterAttribute : ActionFilterAttribute
{
    /// <summary>
    /// OnActionExecutionAsync
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        var services = context.HttpContext.RequestServices;
        var environment = services.GetRequiredService<IWebHostEnvironment>();
        var appSetting = context.HttpContext.RequestServices.GetRequiredService<AppSetting>();

        if (!(!environment.IsDevelopment() || appSetting.IsEnableAuth))
        {
            await next().ConfigureAwait(false);
            return;
        }

        var logger = services.GetRequiredService<ILogger<ApiAuthorizeFilterAttribute>>();

        var routeValues = context.ActionDescriptor.RouteValues;
        var controllerName = routeValues["controller"].ToLower();
        var actionName = routeValues["action"].ToLower();
        var permission =
            $"{appSetting.AuthorizationServer.Service}:{context.HttpContext.Request.Method.ToLower()}:{controllerName}_{actionName}";

        context.HttpContext.Items.Add("policy", permission);

        try
        {
            logger.LogDebug("Getting Policy '{policy}'", permission);

            var policy = GetPolicy(appSetting, permission);

            if (policy is { IsCheck: true } or null)
            {
                var authorizationService = context
                    .HttpContext
                    .RequestServices
                    .GetRequiredService<IAuthorizationService>();

                logger.LogDebug("Checking Permission '{permission}'", permission);

                var result = await authorizationService
                    .AuthorizeAsync(context.HttpContext.User, null, permission)
                    .ConfigureAwait(false);

                if (!result.Succeeded)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Error when checking permission: {message}", e.Message);
            context.Result = new ForbidResult();
            return;
        }

        await next().ConfigureAwait(false);
    }

    private static Policy GetPolicy(AppSetting appSetting, string policy)
    {
        var policyList = appSetting.AuthorizationServer.Policy ?? [];

        return policyList.Count != 0
            ? policyList.Find(
                x => x.Name.Equals(policy, StringComparison.InvariantCultureIgnoreCase)
            )
            : null;
    }
}
