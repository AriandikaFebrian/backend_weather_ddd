// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Dynamic;

namespace NetCa.Application.Common.Mappings;

/// <summary>
/// MappingExtensions
/// </summary>
public static class MappingExtensions
{
    /// <summary>
    /// PaginatedListAsync
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="request"></param>
    /// <param name="meta"></param>
    /// <typeparam name="TDestination"></typeparam>
    /// <returns></returns>
    public static Task<DocumentRootJson<List<TDestination>>> PaginatedListAsync<TDestination>(
        this IQueryable<TDestination> queryable,
        QueryModel request,
        ExpandoObject meta = default
    )
        where TDestination : class =>
        PaginatedList<TDestination>.CreateAsync(queryable.AsNoTracking(), request, meta);

    /// <summary>
    /// ProjectToListAsync
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="configuration"></param>
    /// <typeparam name="TDestination"></typeparam>
    /// <returns></returns>
    public static Task<List<TDestination>> ProjectToListAsync<TDestination>(
        this IQueryable queryable,
        IConfigurationProvider configuration
    )
        where TDestination : class =>
        queryable.ProjectTo<TDestination>(configuration).AsNoTracking().ToListAsync();
}
