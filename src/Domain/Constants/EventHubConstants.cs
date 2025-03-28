// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

namespace NetCa.Domain.Constants;

/// <summary>
/// EventHubConstants
/// </summary>
public abstract class EventHubConstants
{
    /// <summary>
    /// ConsumerList
    /// </summary>
    public static readonly IReadOnlyDictionary<string, (string, bool)> ConsumerJobList =
        new Dictionary<string, (string, bool)> { };
}
