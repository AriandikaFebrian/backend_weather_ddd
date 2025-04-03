// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;
using Microsoft.Extensions.Logging;

namespace NetCa.Application.Common.Models;

/// <summary>
/// Queryable
/// </summary>
public static class Queryable
{
    private static readonly ILogger Logger = AppLoggingExtensions.CreateLogger(nameof(Queryable));
    private static Type _type;

    /// <summary>
    /// Query
    /// </summary>
    /// <param name="source"></param>
    /// <param name="queryModel"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IQueryable<T> Query<T>(this IQueryable<T> source, QueryModel queryModel)
    {
        ArgumentNullException.ThrowIfNull(source);

        _type = typeof(T);

        source = Filter(source, queryModel.GetFiltersParsed());

        source = Sort(source, queryModel.GetSortsParsed());

        if (queryModel.PageNumber < 1)
        {
            queryModel.PageNumber = PaginationConstants.DefaultPageNumber;
        }

        if (queryModel.PageSize < 1)
        {
            queryModel.PageSize = PaginationConstants.DefaultPageSize;
        }

        source = Limit(
            source,
            queryModel.PageNumber ?? PaginationConstants.DefaultPageNumber,
            queryModel.PageSize ?? PaginationConstants.DefaultPageSize
        );

        return source;
    }

    /// <summary>
    /// QueryWithoutLimit
    /// </summary>
    /// <param name="source"></param>
    /// <param name="queryModel"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IQueryable<T> QueryWithoutLimit<T>(
        this IQueryable<T> source,
        QueryModel queryModel
    )
    {
        ArgumentNullException.ThrowIfNull(source);

        _type = typeof(T);

        source = Filter(source, queryModel.GetFiltersParsed());

        source = Sort(source, queryModel.GetSortsParsed());

        return source;
    }

    /// <summary>
    /// Filter
    /// </summary>
    /// <param name="source"></param>
    /// <param name="queryModel"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IQueryable<T> Filter<T>(this IQueryable<T> source, QueryModel queryModel)
    {
        ArgumentNullException.ThrowIfNull(source);

        _type = typeof(T);

        source = Filter(source, queryModel.GetFiltersParsed());

        return source;
    }

    private static IQueryable<T> Filter<T>(IQueryable<T> source, IList<FilterQuery> filter)
    {
        try
        {
            if (filter != null && filter.Any())
            {
                var where = SwitchLogic(filter);

                if (!string.IsNullOrEmpty(where))
                {
                    var values = filter.ToArray();
                    var index = 0;

                    foreach (var item in values)
                    {
                        if (
                            item.Operator.Contains('>')
                            || item.Operator.Contains(">=")
                            || item.Operator.Contains('<')
                            || item.Operator.Contains("<=")
                        )
                        {
                            if (
                                DateTimeOffset.TryParse(
                                    item.Value.ToString(),
                                    CultureInfo.InvariantCulture,
                                    out _
                                )
                            )
                            {
                                var date = DateTimeOffset
                                    .Parse(
                                        item.Value.ToString() ?? string.Empty,
                                        CultureInfo.InvariantCulture
                                    )
                                    .ToUniversalTime();

                                values[index].Value = date.ToString(
                                    ExcelReportConstants.DateFormat,
                                    CultureInfo.InvariantCulture
                                );
                            }

                            index++;
                            continue;
                        }

                        index++;
                    }

                    var result = values.Select(f => f.Value.ToString()?.ToLower()).ToArray();
                    Logger.LogDebug("Filter {Type} with {Where} {Values}", _type, where, result);
                    source = source.Where(where, result);
                }
            }
        }
        catch (Exception e)
        {
            Logger.LogWarning("Failed to filter {Message}", e.Message);
        }

        return source;
    }

    private static string SwitchLogic(IList<FilterQuery> filter)
    {
        string where = null;
        for (var i = 0; i < filter.Count; i++)
        {
            var logic = filter[i].Logic ?? "AND";
            string f;

            if (logic.StartsWith('('))
            {
                f =
                    i == 0
                        ? $"({Transform(logic[1..], filter[i], i)}"
                        : $"{Transform(logic[1..] + " (", filter[i], i)}";
            }
            else if (logic.EndsWith(')'))
            {
                f = $"{Transform(logic[..^1], filter[i], i)})";
            }
            else
            {
                f = Transform(logic, filter[i], i);
            }

            @where = $"{@where} {f}";
        }

        return where;
    }

    private static IQueryable<T> Limit<T>(IQueryable<T> source, int pageNumber, int pageSize)
    {
        Logger.LogDebug(
            "Try to skip {Skip} and take {PageSize}",
            pageSize * (pageNumber - 1),
            pageSize.ToString(CultureInfo.InvariantCulture)
        );

        return source.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
    }

    private static readonly IDictionary<string, string> Operators = new Dictionary<string, string>
    {
        { "eq", "=" },
        { "neq", "!=" },
        { "lt", "<" },
        { "lte", "<=" },
        { "gt", ">" },
        { "gte", ">=" },
        { "startswith", "StartsWith" },
        { "endswith", "EndsWith" },
        { "contains", "Contains" },
        { "doesnotcontain", "Contains" },
        { "==", "=" },
        { "!=", "!=" },
        { "<", "<" },
        { "<=", "<=" },
        { ">", ">" },
        { ">=", ">=" },
        { "_=", "StartsWith" },
        { "=_", "EndsWith" },
        { "@=", "Contains" },
        { "!@=", "Contains" }
    };

    /// <summary>
    /// Transform
    /// </summary>
    /// <param name="logic"></param>
    /// <param name="filter"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private static string Transform(string logic, FilterQuery filter, int index)
    {
        if (
            filter.Value == null
            || string.IsNullOrEmpty(filter.Field)
            || string.IsNullOrEmpty(filter.Value.ToString())
        )
        {
            return null;
        }

        try
        {
            if (filter.Operator != null)
            {
                return TransformLogic(
                    Operators[filter.Operator.ToLower()],
                    logic,
                    filter,
                    index,
                    filter.Value.ToString()
                );
            }
        }
        catch (Exception e)
        {
            Logger.LogWarning(
                "Operator {Operator} not part of the Dictionary: {Message}",
                filter.Operator,
                e.Message
            );
        }

        return null;
    }

    private static string TransformLogic(
        string comparison,
        string logic,
        FilterQuery filter,
        int index,
        string value
    )
    {
        var parameter =
            index > 0
                ? "{0} ({1} != null && {1}.ToLower() {2} @{3})"
                : "({0} != null && {0}.ToLower() {1} @{2})";

        if (new List<string> { "doesnotcontain", "!@=" }.Contains(filter.Operator))
        {
            parameter =
                index > 0
                    ? "{0} ({1} != null && !{1}.{2}(@{3}))"
                    : "({0} != null && !{0}.{1}(@{2}))";
        }

        if (
            int.TryParse(value, out _)
            || double.TryParse(value, out _)
            || float.TryParse(value, out _)
            || bool.TryParse(value, out _)
        )
        {
            parameter =
                index > 0 ? "{0} ({1} != null && {1} {2} @{3})" : "({0} != null && {0} {1} @{2})";
        }
        else if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, out _))
        {
            parameter = index > 0 ? "{0} ({1} {2} @{3})" : "({0}{1} @{2})";
        }

        return index > 0
            ? string.Format(
                CultureInfo.InvariantCulture,
                parameter,
                logic,
                filter.Field,
                comparison,
                index
            )
            : string.Format(
                CultureInfo.InvariantCulture,
                parameter,
                filter.Field,
                comparison,
                index
            );
    }

    /// <summary>
    /// Sorting query by column
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sort"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static IQueryable<T> Sort<T>(this IQueryable<T> source, IReadOnlyCollection<Sort> sort)
    {
        if (sort == null || sort.Count == 0)
        {
            return source;
        }

        try
        {
            var ordering = string.Join(",", sort.Select(s => $"{s.Field} {s.Direction}"));

            Logger.LogDebug("Try to sort {Type} with {Ordering}", _type, ordering);

            return source.OrderBy(ordering);
        }
        catch (ParseException e)
        {
            Logger.LogWarning(
                "sortBy include field not part of the {Type}: {Message}",
                _type,
                e.Message
            );
        }

        return source;
    }
}
