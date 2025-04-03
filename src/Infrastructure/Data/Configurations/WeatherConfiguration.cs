using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCa.Infrastructure.Data.Configurations;
using NetCa.Domain.Entities; // ✅ Pastikan ini di-import

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

        builder.Property(x => x.Temperature) // ✅ Decimal
            .HasColumnType("decimal(10,2)");

        builder.Property(x => x.Humidity)
            .HasColumnType("varchar(50)") // ✅ Lebih masuk akal
            .HasMaxLength(50);

        builder.Property(x => x.WindSpeed) // ✅ Decimal
            .HasColumnType("decimal(10,2)");

        builder.Property(x => x.CloudCoverage) // ✅ Integer
            .HasColumnType("int");

        builder.Property(x => x.Country) // ✅ String
            .HasColumnType("varchar(100)")
            .HasMaxLength(100);

        builder.Property(x => x.Timestamp) // ✅ DateTime
            .HasColumnType("datetime");

        base.Configure(builder);
    }
}
