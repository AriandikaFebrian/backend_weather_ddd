// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using NetCa.Domain.Common;

namespace NetCa.Domain.Entities;

/// <summary>
/// TodoItem
/// </summary>
public record TodoItem : BaseAuditableEntity
{
    /// <summary>
    /// Gets or sets Title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets StartDate
    /// </summary>
    public DateOnly StartDate { get; set; }
}
