// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;

namespace NetCa.Api.Infrastructures.Extensions;

/// <summary>
/// FormValueRequiredAttribute
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FormValueRequiredAttribute"/> class.
/// </remarks>
/// <param name="_name"></param>
public sealed class FormValueRequiredAttribute(string _name) : ActionMethodSelectorAttribute
{
    /// <summary>
    /// IsValidForRequest
    /// </summary>
    /// <param name="routeContext"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
    {
        if (string.Equals(routeContext.HttpContext.Request.Method, "GET", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(routeContext.HttpContext.Request.Method, "HEAD", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(routeContext.HttpContext.Request.Method, "DELETE", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(routeContext.HttpContext.Request.Method, "TRACE", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (string.IsNullOrEmpty(routeContext.HttpContext.Request.ContentType))
        {
            return false;
        }

        if (!routeContext.HttpContext.Request.ContentType.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return !string.IsNullOrEmpty(routeContext.HttpContext.Request.Form[_name]);
    }
}
