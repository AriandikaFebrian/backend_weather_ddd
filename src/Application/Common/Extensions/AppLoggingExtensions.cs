// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace NetCa.Application.Common.Extensions;

/// <summary>
/// Shared logger
/// </summary>
public static class AppLoggingExtensions
{
    /// <summary>
    /// Gets or sets loggerFactory
    /// </summary>
    /// <value></value>
    public static ILoggerFactory LoggerFactory { get; set; }

    /// <summary>
    /// CreateLogger
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ILogger<T> CreateLogger<T>() => LoggerFactory.CreateLogger<T>();

    /// <summary>
    /// CreateLogger
    /// </summary>
    /// <param name="categoryName"></param>
    /// <returns></returns>
    public static ILogger CreateLogger(string categoryName) => LoggerFactory.CreateLogger(categoryName);
}
