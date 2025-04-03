// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NetCa.Infrastructure.Data.Configurations;

/// <summary>
/// TodoItemConfiguration
/// </summary>
public class TodoItemConfiguration : AuditTableConfiguration<TodoItem>
{
    /// <summary>
    /// Configure TodoItem
    /// </summary>
    /// <param name="builder"></param>
    public override void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.Property(x => x.Title)
            .HasColumnType("varchar(100)")
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasColumnType("varchar(255)")
            .HasMaxLength(255);

        builder.Property(x => x.StartDate)
            .HasColumnType("date");

        base.Configure(builder);
    }
}
