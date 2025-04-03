// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------
using System.Globalization;

namespace NetCa.Application.Common.Dtos;

/// <summary>
/// WeatherDto
/// </summary>
public class WeatherDto
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
    /// Gets or sets TImeStamp
    /// </summary>
    public DateTime Timestamp { get; set; }
}