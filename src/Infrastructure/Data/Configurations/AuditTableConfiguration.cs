// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCa.Domain.Common;

namespace NetCa.Infrastructure.Data.Configurations;

/// <summary>
/// AuditTableConfiguration
/// </summary>
/// <typeparam name="TBase"></typeparam>
public abstract class AuditTableConfiguration<TBase> : IEntityTypeConfiguration<TBase>
    where TBase : BaseAuditableEntity
{
    /// <summary>
    /// Configure for all entities
    /// </summary>
    /// <param name="builder"></param>
    public virtual void Configure(EntityTypeBuilder<TBase> builder)
    {
        builder.HasQueryFilter(e => e.IsActive.Value);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnType("uuid")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.CreatedBy)
            .HasColumnType("varchar(150)")
            .HasMaxLength(150);

        builder.Property(e => e.CreatedDate)
            .HasDefaultValueSql("now()");

        builder.Property(e => e.UpdatedBy)
            .HasColumnType("varchar(150)")
            .HasMaxLength(150);

        builder.Property(e => e.UpdatedDate)
            .HasDefaultValueSql("now()");

        builder.Property(e => e.IsActive)
            .HasColumnType("bool")
            .HasDefaultValue(true);

        builder.Property(e => e.IsDeleted)
            .HasColumnType("bool")
            .HasDefaultValue(false);

        builder.HasIndex(e => e.IsActive);
        builder.HasIndex(e => e.IsDeleted);

        builder.HasQueryFilter(e => !e.IsDeleted.Value);
    }
}
