// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Dynamic;

namespace NetCa.Application.Common.Models;

/// <summary>
/// PaginatedList
/// </summary>
/// <typeparam name="T"></typeparam>
/// <returns></returns>
public static class PaginatedList<T>
{
    /// <summary>
    /// Gets a value indicating whether HasNextPage
    /// </summary>
    /// <value></value>
    /// <param name="source"></param>
    /// <param name="request"></param>
    /// <param name="meta"></param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task<DocumentRootJson<List<T>>> CreateAsync(
        IQueryable<T> source,
        QueryModel request,
        dynamic meta
    )
    {
        var futureCount = source.Filter(request).DeferredCount().FutureValue();
        var futureEntities = source.Query(request).Future();

        if (source is IQueryable<IBaseMetaDto>)
        {
            var query = source as IQueryable<IBaseMetaDto>;

            var activeCountFuture = query
                .Where(x => x.IsActive.Value)
                .DeferredCount()
                .FutureValue();
            var inActiveCountFuture = query
                .Where(x => !x.IsActive.Value)
                .DeferredCount()
                .FutureValue();

            meta ??= new ExpandoObject();
            meta.totalActiveItems = activeCountFuture.Value;
            meta.totalInActiveItems = inActiveCountFuture.Value;
        }

        var items = futureEntities.ToList();
        var count = futureCount.Value;

        return JsonApiExtensions.ToJsonApiPaginated(
            items,
            meta ?? new ExpandoObject(),
            count,
            request.PageNumber ?? PaginationConstants.DefaultPageNumber,
            request.PageSize ?? PaginationConstants.DefaultPageSize
        );
    }
}
