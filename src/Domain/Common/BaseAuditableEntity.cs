// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

namespace NetCa.Domain.Common;

/// <summary>
/// BaseAuditableEntity
/// </summary>
public abstract record BaseAuditableEntity : BaseEntity, IBaseAuditableEntity
{
    /// <summary>
    /// Gets or sets createdBy
    /// </summary>
    public string CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets createdDate
    /// </summary>
    public DateTimeOffset? CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets updatedBy
    /// </summary>
    public string UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets updatedDate
    /// </summary>
    public DateTimeOffset? UpdatedDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether isActive
    /// </summary>
    public bool? IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether isDeleted
    /// </summary>
    public bool? IsDeleted { get; set; } = false;

    /// <summary>
    /// Gets or sets a unique identifier for tracking
    /// </summary>
    public Guid TrackingId { get; set; } = Guid.NewGuid();
}
