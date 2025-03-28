// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace NetCa.Api.Infrastructures.Extensions;

/// <summary>
/// IEndpointRouteBuilderExtensions
/// </summary>
public static class IEndpointRouteBuilderExtensions
{
    /// <summary>
    /// MapGet
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="handler"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapGet(
        this IEndpointRouteBuilder builder,
        Delegate handler,
        [StringSyntax("Route")] string pattern = ""
    )
    {
        Guard.Against.AnonymousMethod(handler);

        builder.MapGet(pattern, handler).WithName(handler.Method.Name);

        return builder;
    }

    /// <summary>
    /// MapPost
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="handler"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapPost(
        this IEndpointRouteBuilder builder,
        Delegate handler,
        [StringSyntax("Route")] string pattern = ""
    )
    {
        Guard.Against.AnonymousMethod(handler);

        builder.MapPost(pattern, handler).WithName(handler.Method.Name);

        return builder;
    }

    /// <summary>
    /// MapPut
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="handler"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapPut(
        this IEndpointRouteBuilder builder,
        Delegate handler,
        [StringSyntax("Route")] string pattern
    )
    {
        Guard.Against.AnonymousMethod(handler);

        builder.MapPut(pattern, handler).WithName(handler.Method.Name);

        return builder;
    }

    /// <summary>
    /// MapDelete
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="handler"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapDelete(
        this IEndpointRouteBuilder builder,
        Delegate handler,
        [StringSyntax("Route")] string pattern
    )
    {
        Guard.Against.AnonymousMethod(handler);

        builder.MapDelete(pattern, handler).WithName(handler.Method.Name);

        return builder;
    }
}
