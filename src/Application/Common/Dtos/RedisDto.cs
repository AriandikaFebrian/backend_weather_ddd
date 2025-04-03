// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

namespace NetCa.Application.Common.Dtos;

/// <summary>
/// RedisDto
/// </summary>
public record RedisDto
{
    /// <summary>
    /// Gets or sets code
    /// </summary>
    /// <value></value>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets desc
    /// </summary>
    /// <value></value>
    public string Value { get; set; }
}
