using NetCa.Domain.Common;

namespace NetCa.Domain.Entities;

/// <summary>
/// Weather
/// </summary>
public record Weather : BaseAuditableEntity
{

    public Weather()
    {
        Id = Guid.NewGuid();
    }

    public string City { get; set; }
    public string WeatherDescription { get; set; }
    public decimal Temperature { get; set; }
    public string Humidity { get; set; }
    public decimal WindSpeed { get; set; }
    public int CloudCoverage { get; set; }
    public string Country { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}