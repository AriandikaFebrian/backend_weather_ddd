// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NetCa.Application.Common.Extensions;

/// <summary>
/// JsonExtensions
/// </summary>
public static class JsonExtensions
{
    private static readonly ILogger _logger = AppLoggingExtensions.CreateLogger("JsonExtensions");

    /// <summary>
    /// ErrorSerializerSettings
    /// </summary>
    /// <returns></returns>
    public static JsonSerializerSettings ErrorSerializerSettings()
    {
        return new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };
    }

    /// <summary>
    /// SerializerSettings
    /// </summary>
    /// <returns></returns>
    public static JsonSerializerSettings SerializerSettings()
    {
        return new JsonSerializerSettings
        {
            Error = HandleDeserializationError(),
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new DefaultContractResolver()
        };
    }

    /// <summary>
    /// SyncSerializerSettings
    /// </summary>
    /// <returns></returns>
    public static JsonSerializerSettings SyncSerializerSettings()
    {
        return new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver(),
            Error = HandleDeserializationError(),
            Formatting = Formatting.Indented
        };
    }

    /// <summary>
    /// HandleDeserializationError
    /// </summary>
    /// <returns></returns>
    public static EventHandler<ErrorEventArgs> HandleDeserializationError()
    {
        return HandleErrorParsing;
    }

    private static void HandleErrorParsing(object sender, ErrorEventArgs errorArgs)
    {
        var currentError = errorArgs.ErrorContext.Error.Message;

        _logger?.LogWarning("Error when serialize value: {message}", currentError);

        errorArgs.ErrorContext.Handled = true;
    }
}
