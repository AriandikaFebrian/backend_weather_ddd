using System.Globalization;

namespace NetCa.Application.Common.Dtos;

/// <summary>
/// WeatherDetailDto
/// </summary>
public class WeatherDetailDto
{
    /// <summary>
    /// Gets or sets city
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// Gets or sets WeatherDescription
    /// </summary>
    public string WeatherDescription { get; set; }

    /// <summary>
    /// Gets or sets Temprature
    /// </summary>
    public decimal Temperature { get; set; }

    /// <summary>
    /// Gets or sets Humidity
    /// </summary>
    public string Humidity { get; set; }

    /// <summary>
    /// Gets or sets WindSpeed
    /// </summary>
    public string WindSpeed { get; set; }

    /// <summary>
    /// Gets or sets CloudCoverage
    /// </summary>
    public string CloudCoverage { get; set; }
}
