// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace NetCa.Application.Common.Models;

/// <summary>
/// Model for receive sort, filter and paging
/// </summary>
public record QueryModel
{
    private static readonly ILogger Logger = AppLoggingExtensions.CreateLogger("QueryModel");

    /// <summary>
    /// Gets or sets Searching Key for Global Search
    /// </summary>
    public string SearchValue { get; set; }

    /// <summary>
    /// Gets or sets filters
    /// </summary>
    public string Filters { get; set; }

    /// <summary>
    /// Gets or sets sorts
    /// </summary>
    public string Sorts { get; set; }

    /// <summary>
    /// Gets or sets number of req page
    /// </summary>
    /// <value>number of req page</value>
    /// <example>1</example>
    public int? PageNumber { get; set; } = PaginationConstants.DefaultPageNumber;

    /// <summary>
    /// Gets or sets limit data each page
    /// </summary>
    /// <value>limit data each page</value>
    /// <example>10</example>
    public int? PageSize { get; set; } = PaginationConstants.DefaultPageSize;

    /// <summary>
    /// Parsing Filters to list FilterQuery model
    /// </summary>
    /// <returns></returns>
    public List<FilterQuery> GetFiltersParsed()
    {
        if (Filters == null)
        {
            return [];
        }

        var value = new List<FilterQuery>();

        foreach (var filter in FilterAndSortSeparatorConstants.EscapedCommaPattern().Split(Filters))
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                continue;
            }

            if (filter.StartsWith('('))
            {
                CheckStartWith(filter, value);
            }
            else
            {
                NotCheckStartWith(filter, value);
            }
        }

        return value;
    }

    private static void CheckStartWith(string filter, List<FilterQuery> value)
    {
        var filterOpAndVal = filter[(filter.LastIndexOf(')') + 1)..];
        var sub = filter.Replace(filterOpAndVal, "").Replace("(", "").Replace(")", "");

        var subFilters = FilterAndSortSeparatorConstants.EscapedPipePattern().Split(sub);

        for (var i = 0; i < subFilters.Length; i++)
        {
            var filterSplit = filterOpAndVal
                .Split(
                    FilterAndSortSeparatorConstants.Operators,
                    StringSplitOptions.RemoveEmptyEntries
                )
                .Select(t => t.Trim())
                .ToArray();

            var logic = "OR";

            if (i == 0)
            {
                logic = "(AND";
            }
            else if (i == subFilters.Length - 1)
            {
                logic = "OR)";
            }

            value.Add(
                new FilterQuery
                {
                    Field = subFilters[i],
                    Operator = filterSplit[0],
                    Value = filterSplit[1],
                    Logic = logic
                }
            );

            Logger.LogDebug(
                "Filter = {Filter1} {Filter2} {Filter3}",
                subFilters[i],
                filterSplit[0],
                filterSplit[1]
            );
        }
    }

    private static void NotCheckStartWith(string filter, List<FilterQuery> value)
    {
        var filterSplit = filter
            .Split(FilterAndSortSeparatorConstants.Operators, StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .ToArray();

        if (filterSplit[1].StartsWith('('))
        {
            var subFilters = FilterAndSortSeparatorConstants
                .EscapedPipePattern()
                .Split(filterSplit[1][1..filterSplit[1].IndexOf(')')]);

            for (var i = 0; i < subFilters.Length; i++)
            {
                var logic = "OR";

                if (i == 0)
                {
                    logic = "(AND";
                }
                else if (i == subFilters.Length - 1)
                {
                    logic = "OR)";
                }

                value.Add(
                    new FilterQuery
                    {
                        Field = filterSplit[0],
                        Operator =
                            Array.Find(FilterAndSortSeparatorConstants.Operators, filter.Contains)
                            ?? "==",
                        Value = subFilters[i],
                        Logic = logic
                    }
                );

                Logger.LogDebug(
                    "Filter = {F1} {Op} {Fs}",
                    filterSplit[0],
                    Array.Find(FilterAndSortSeparatorConstants.Operators, filter.Contains) ?? "==",
                    subFilters[i]
                );
            }

            return;
        }

        value.Add(
            new FilterQuery
            {
                Field = filterSplit[0],
                Operator =
                    Array.Find(FilterAndSortSeparatorConstants.Operators, filter.Contains) ?? "==",
                Value = filterSplit[1],
                Logic = "AND"
            }
        );

        Logger.LogDebug(
            "Filter = {F1} {Op} {Fs}",
            filterSplit[0],
            Array.Find(FilterAndSortSeparatorConstants.Operators, filter.Contains) ?? "==",
            filterSplit[1]
        );
    }

    /// <summary>
    /// Parsing Sorts to Sort model
    /// </summary>
    /// <returns></returns>
    public List<Sort> GetSortsParsed()
    {
        if (Sorts == null)
        {
            return [];
        }

        var value = new List<Sort>();

        foreach (var sort in FilterAndSortSeparatorConstants.EscapedCommaPattern().Split(Sorts))
        {
            if (string.IsNullOrWhiteSpace(sort))
            {
                continue;
            }

            value.Add(
                sort[..1] == "-"
                    ? new Sort { Field = sort[1..], Direction = "DESC" }
                    : new Sort { Field = sort, Direction = "ASC" }
            );
        }

        return value;
    }
}

/// <summary>
/// Sort
/// </summary>
public class Sort
{
    /// <summary>
    /// Gets or sets field
    /// </summary>
    /// <value></value>
    public string Field { get; set; }

    /// <summary>
    /// Gets or sets direction
    /// </summary>
    /// <value></value>
    public string Direction { get; set; }
}

/// <summary>
/// FilterQuery
/// </summary>
public class FilterQuery
{
    /// <summary>
    /// Gets or sets field
    /// </summary>
    /// <value>Field name to filter</value>
    /// <example>CreatedBy</example>
    public string Field { get; set; }

    /// <summary>
    /// Gets or sets operator eq,neq,lt,lte,gt,gte,startswith,endswith,contains,doesnotcontain
    /// </summary>
    /// <value>logical operator</value>
    /// <example>eq</example>
    public string Operator { get; set; }

    /// <summary>
    /// Gets or sets value
    /// </summary>
    /// <value>value to search</value>
    /// <example>xx</example>
    public object Value { get; set; }

    /// <summary>
    /// Gets or sets logic AND OR
    /// </summary>
    /// <value>logical operator</value>
    /// <example>AND</example>
    public string Logic { get; set; }
}
