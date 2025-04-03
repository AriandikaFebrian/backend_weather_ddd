// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

namespace NetCa.Domain.Common;

/// <summary>
/// IBaseAuditableEntity
/// </summary>
public interface IBaseAuditableEntity
{
    /// <summary>
    /// Gets or sets the user who created the entity
    /// </summary>
    public string CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date when the entity was created
    /// </summary>
    public DateTimeOffset? CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the user who last updated the entity
    /// </summary>
    public string UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date when the entity was last updated
    /// </summary>
    public DateTimeOffset? UpdatedDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is active
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is deleted
    /// </summary>
    public bool? IsDeleted { get; set; }
}
