using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCa.Infrastructure.Data.Configurations;
using NetCa.Domain.Entities;

namespace NetCa.Infrastructure.Data.Configurations;

public class WeatherConfiguration : AuditTableConfiguration<Weather>
{
    public override void Configure(EntityTypeBuilder<Weather> builder)
    {
        builder.Property(x => x.City)
            .HasColumnType("varchar(100)")
            .HasMaxLength(100);

        builder.Property(x => x.WeatherDescription)
            .HasColumnType("varchar(255)")
            .HasMaxLength(255);

        builder.Property(x => x.Temperature)
            .HasColumnType("decimal(10,2)");

        builder.Property(x => x.Humidity)
            .HasColumnType("varchar(50)")
            .HasMaxLength(50);

        builder.Property(x => x.WindSpeed)
            .HasColumnType("decimal(10,2)");

        builder.Property(x => x.CloudCoverage)
            .HasColumnType("int");

        builder.Property(x => x.Country)
            .HasColumnType("varchar(100)")
            .HasMaxLength(100);

        builder.Property(x => x.Timestamp)
            .HasColumnType("timestamp");

        base.Configure(builder);
    }
}
