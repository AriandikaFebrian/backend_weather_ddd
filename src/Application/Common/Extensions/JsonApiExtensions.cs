// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Dynamic;
using JsonApiSerializer;
using JsonApiSerializer.JsonApi;
using JsonApiSerializer.JsonApi.WellKnown;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NetCa.Application.Common.Extensions;

/// <summary>
/// JsonApiExtensions
/// </summary>
public static class JsonApiExtensions
{
    /// <summary>
    /// ToJsonApiProjectTo
    /// </summary>
    /// <param name="data"></param>
    /// <param name="meta"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<DocumentRootJson<List<T>>> ToJsonApiProjectTo<T>(
        IQueryable<T> data,
        dynamic meta,
        int pageNumber = 1,
        int pageSize = 1
    )
    {
        var totalItems = await data.CountAsync().ConfigureAwait(false);
        var items = await data.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync()
            .ConfigureAwait(false);
        var totalPages = totalItems > 0 ? (int)Math.Ceiling(totalItems / (double)pageSize) : 0;
        var hasPreviousPage = pageNumber > 1;
        var hasNextPage = pageNumber < totalPages;
        var nextPageNumber = hasNextPage ? pageNumber + 1 : totalPages;
        var previousPageNumber = hasPreviousPage ? pageNumber - 1 : 1;

        meta.totalItems = totalItems;
        meta.pageNumber = pageNumber;
        meta.pageSize = pageSize;
        meta.totalPages = totalPages;
        meta.hasPreviousPage = hasPreviousPage;
        meta.hasNextPage = hasNextPage;
        meta.nextPageNumber = nextPageNumber;
        meta.previousPageNumber = previousPageNumber;

        return new DocumentRootJson<List<T>>
        {
            Data = items,
            Meta = meta,
            Status = new Status()
        };
    }

    /// <summary>
    /// ToJsonApiPaginated
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="meta"></param>
    /// <param name="totalItems"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public static DocumentRootJson<T> ToJsonApiPaginated<T>(
        T data,
        dynamic meta,
        int totalItems = 0,
        int pageNumber = 1,
        int pageSize = 1
    )
    {
        var totalPages = 1;

        if (pageSize > 0)
        {
            totalPages =
                totalItems > 0 ? (int)Math.Ceiling(totalItems / (double)pageSize) : totalPages;
        }

        var hasPreviousPage = pageNumber > 1;
        var hasNextPage = pageNumber < totalPages;
        var nextPageNumber = hasNextPage ? pageNumber + 1 : totalPages;
        var previousPageNumber = hasPreviousPage ? pageNumber - 1 : 1;

        meta.totalItems = totalItems;
        meta.pageNumber = pageNumber;
        meta.pageSize = pageSize;
        meta.totalPages = totalPages;
        meta.hasPreviousPage = hasPreviousPage;
        meta.hasNextPage = hasNextPage;
        meta.nextPageNumber = nextPageNumber;
        meta.previousPageNumber = previousPageNumber;

        return new DocumentRootJson<T>
        {
            Data = data,
            Meta = meta,
            Status = new Status()
        };
    }

    /// <summary>
    /// ToJsonApi
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public static DocumentRootJson<T> ToJsonApi<T>(T data)
    {
        return new DocumentRootJson<T>
        {
            Data = data,
            Meta = new Meta(),
            Status = new Status()
        };
    }

    /// <summary>
    /// ToJsonApi
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    public static DocumentRootJson<T> ToJsonApi<T>(T data, Status status)
    {
        return new DocumentRootJson<T>
        {
            Data = data,
            Meta = new Meta(),
            Status = status
        };
    }

    /// <summary>
    /// SyncSerializerSettings
    /// </summary>
    /// <returns></returns>
    public static JsonApiSerializerSettings SyncSerializerSettings()
    {
        return new JsonApiSerializerSettings { Formatting = Formatting.Indented };
    }

    /// <summary>
    /// SerializerSettings
    /// </summary>
    /// <returns></returns>
    public static JsonApiSerializerSettings SerializerSettings()
    {
        return new JsonApiSerializerSettings();
    }
}

/// <summary>
/// DocumentRootJson
/// </summary>
/// <typeparam name="TData"></typeparam>
public class DocumentRootJson<TData> : IDocumentRoot<TData>
{
    /// <summary>
    /// Gets or sets data
    /// </summary>
    /// <value>
    /// Data
    /// </value>
    [JsonProperty(Order = 2)]
    public TData Data { get; set; }

    /// <summary>
    /// Gets or sets included
    /// </summary>
    /// <value>
    /// Included
    /// </value>
    [JsonProperty(Order = 3)]
    public List<JObject> Included { get; set; }

    /// <summary>
    /// Gets or sets meta
    /// </summary>
    /// <value></value>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 1)]
    public dynamic Meta { get; set; } = new ExpandoObject();

    /// <summary>
    /// Gets or sets responseTime in ms
    /// </summary>
    /// <value>
    /// ResponseTime in ms
    /// </value>
    [JsonProperty(Order = 1000)]
    public long ResponseTime { get; set; }

    /// <summary>
    /// Gets or sets status
    /// </summary>
    /// <value>
    /// Status
    /// </value>
    [JsonProperty(Order = 10000)]
    public Status Status { get; set; }
}

/// <summary>
/// Status
/// </summary>
public class Status
{
    /// <summary>
    /// Gets or sets code
    /// </summary>
    /// <value>
    /// Code
    /// </value>
    public int Code { get; set; }

    /// <summary>
    /// Gets or sets desc
    /// </summary>
    /// <value>
    /// Desc
    /// </value>
    public object Desc { get; set; }
}
