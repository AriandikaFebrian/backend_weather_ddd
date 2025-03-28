// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------
using System.Globalization;
using NetCa.Application.Common.Mappings;
using NetCa.Domain.Common;

namespace NetCa.Application.Common.Vms;

/// <summary>
/// WeatherVm
/// </summary>
public record WeatherVm : BaseAuditableEntity, IMapFrom<Weather>
{

    /// <summary>
    /// Gets or sets City
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// Gets or sets WeatherDescription
    /// </summary>
    public string WeatherDescription { get; set; }

    /// <summary>
    /// Gets or sets Temperature
    /// </summary>
    public decimal Temperature { get; set; }

    /// <summary>
    /// Gets or sets Humidity
    /// </summary>
    public string Humidity { get; set; }

    /// <summary>
    /// Gets or sets WindSpeed
    /// </summary>
    public decimal WindSpeed { get; set; }

    /// <summary>
    /// Gets or sets CloudCoverage
    /// </summary>
    public int CloudCoverage { get; set; }

    /// <summary>
    /// Gets or sets Country
    /// </summary>
    public string Country { get; set; }

    /// <summary>
    /// Gets or sets Timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    ///  Gets
    /// </summary>
    public string Date => Timestamp.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    /// <summary>
    /// Mapping
    /// </summary>
    /// <param name="profile"></param>
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Weather, WeatherVm>();
    }
    
}
