// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace NetCa.Api.Infrastructures.Extensions;

/// <summary>
/// ControllerExtensions
/// </summary>
public static class ControllerExtensions
{
    /// <summary>
    /// GetControllerList
    /// </summary>
    /// <returns></returns>
    public static List<ControllerListDto> GetControllerList()
    {
        var controllerActionList = typeof(Program)
            .Assembly
            .GetTypes()
            .Where(type =>
            {
                var check1 = typeof(ControllerBase).IsAssignableFrom(type);

                if (!check1)
                {
                    return check1;
                }

                var check2 = type.GetCustomAttributes<ApiDevelopmentFilterAttribute>().Any();

                if (check2)
                {
                    return false;
                }

                var check3 = type.GetCustomAttributes<ApiAuthorizeFilterAttribute>().ToList();

                if (check3.Count == 0)
                {
                    return false;
                }

                return true;
            })
            .SelectMany(
                type =>
                    type.GetMethods(
                            BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public
                        )
                        .Where(
                            x =>
                                !x.GetCustomAttributes<CompilerGeneratedAttribute>().Any()
                                && x.GetCustomAttributes<SwaggerResponseAttribute>().Any()
                        )
                        .Select(x =>
                        {
                            var apiVersion = type.GetCustomAttribute<ApiVersionAttribute>()
                                .Versions[0]
                                .ToString()
                                .Replace(".0", string.Empty);
                            var urlTemplate = type.GetCustomAttribute<RouteAttribute>().Template;

                            var method = string.Empty;
                            var urlExt = string.Empty;
                            var groups = new List<string>();

                            if (
                                x.GetCustomAttribute<HttpGetAttribute>() is var httpAttr
                                && httpAttr != null
                            )
                            {
                                method = "GET";
                                urlExt = httpAttr.Template;
                            }
                            else if (
                                x.GetCustomAttribute<HttpPostAttribute>() is var httpAttr1
                                && httpAttr1 != null
                            )
                            {
                                method = "POST";
                                urlExt = httpAttr1.Template;
                            }
                            else if (
                                x.GetCustomAttribute<HttpPutAttribute>() is var httpAttr2
                                && httpAttr2 != null
                            )
                            {
                                method = "PUT";
                                urlExt = httpAttr2.Template;
                            }
                            else if (
                                x.GetCustomAttribute<HttpDeleteAttribute>() is var httpAttr3
                                && httpAttr3 != null
                            )
                            {
                                method = "DELETE";
                                urlExt = httpAttr3.Template;
                            }
                            else if (
                                x.GetCustomAttribute<HttpPatchAttribute>() is var httpAttr4
                                && httpAttr4 != null
                            )
                            {
                                method = "PATCH";
                                urlExt = httpAttr4.Template;
                            }

                            if (
                                x.GetCustomAttribute<ApiAuthorizeGroupAttribute>() is var groupAttr
                                && groupAttr != null
                            )
                            {
                                groups = [.. groupAttr.Group];
                            }

                            var roleLevel =
                                x.GetCustomAttribute<ApiAuthorizeRoleLevelAttribute>()?.Level ?? [];

                            urlExt = string.IsNullOrEmpty(urlExt) ? string.Empty : $"/{urlExt}";

                            return new ControllerListDto
                            {
                                Id = Guid.NewGuid(),
                                Controller = type.Name.Replace("Controller", string.Empty),
                                Action = x.Name,
                                Url =
                                    $"/{urlTemplate.Replace("{version:apiVersion}", apiVersion)}{urlExt}",
                                Method = method,
                                Groups = groups,
                                RoleLevel = [.. roleLevel],
                            };
                        })
            )
            .OrderBy(x => x.Controller)
            .ThenBy(x => x.Action)
            .ToList();

        return controllerActionList;
    }
}
