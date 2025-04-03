// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

namespace NetCa.Application.Common.Dtos;

/// <summary>
/// BaseAuditableEntityDto
/// </summary>
public record BaseAuditableEntityDto
{
    /// <summary>
    /// Gets or sets Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets createdBy
    /// </summary>
    /// <value></value>
    public string CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets createdDate
    /// </summary>
    /// <value></value>
    public DateTimeOffset? CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets updatedBy
    /// </summary>
    /// <value></value>
    public string UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets updatedDate
    /// </summary>
    /// <value></value>
    public DateTimeOffset? UpdatedDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether isActive
    /// </summary>
    /// <value></value>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether isDeleted
    /// </summary>
    /// <value></value>
    public bool? IsDeleted { get; set; }
}
