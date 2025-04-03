// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NetCa.Infrastructure.Data.Configurations;

/// <summary>
/// ChangelogConfiguration
/// </summary>
public class ChangelogConfiguration : IEntityTypeConfiguration<Changelog>
{
    /// <summary>
    /// Configure Changelog
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<Changelog> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnType("uuid")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.TableName)
            .HasColumnType("varchar(50)")
            .HasMaxLength(50);

        builder.Property(e => e.Method)
            .HasColumnType("varchar(6)")
            .HasMaxLength(6);

        builder.Property(e => e.KeyValues)
            .HasColumnType("varchar(100)")
            .HasMaxLength(100);

        builder.Property(e => e.NewValues)
            .HasColumnType("text");

        builder.Property(e => e.OldValues)
            .HasColumnType("text");

        builder.Property(e => e.ChangeBy)
            .HasColumnType("varchar(150)")
            .HasMaxLength(150);

        builder.Property(e => e.ChangeDate)
            .HasDefaultValueSql("now()");
    }
}
