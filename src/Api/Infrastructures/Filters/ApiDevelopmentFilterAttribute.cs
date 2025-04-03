// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NetCa.Api.Infrastructures.Filters;

/// <summary>
/// ApiDevelopmentFilterAttribute
/// </summary>
public class ApiDevelopmentFilterAttribute : ActionFilterAttribute
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
        var logger = context
            .HttpContext
            .RequestServices
            .GetRequiredService<ILogger<ApiDevelopmentFilterAttribute>>();
        var environment = context
            .HttpContext
            .RequestServices
            .GetRequiredService<IWebHostEnvironment>();
        var actionDescriptor = context.ActionDescriptor;
        var ctrl = actionDescriptor.RouteValues["controller"].NullSafeToLower();
        var action = actionDescriptor.RouteValues["action"].NullSafeToLower();
        var env = environment.IsProduction();

        logger.LogDebug("Accessing {Ctrl} {Action}", ctrl, action);

        if (!env)
        {
            await next().ConfigureAwait(false);
        }
        else
        {
            context.Result = new NotFoundResult();
        }
    }
}
