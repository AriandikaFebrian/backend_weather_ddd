using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NetCa.Application.Common.Dtos;

/// <summary>
/// Model untuk mapping response dari OpenWeatherMap API
/// </summary>
public class OpenWeatherMapResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("weather")]
    public List<WeatherInfo> Weather { get; set; } = new();

    [JsonPropertyName("main")]
    public MainInfo Main { get; set; } = new();

    [JsonPropertyName("wind")]
    public WindInfo Wind { get; set; } = new();

    [JsonPropertyName("clouds")]
    public CloudInfo Clouds { get; set; } = new();

    [JsonPropertyName("sys")]
    public SysInfo Sys { get; set; } = new();
}

/// <summary>
/// Data cuaca umum (deskripsi)
/// </summary>
public class WeatherInfo
{
    [JsonPropertyName("description")]
    public string Description { get; set; }
}

/// <summary>
/// Data utama cuaca (suhu, tekanan, kelembapan)
/// </summary>
public class MainInfo
{
    [JsonPropertyName("temp")]
    public decimal Temp { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }
}

/// <summary>
/// Data angin (kecepatan, arah)
/// </summary>
public class WindInfo
{
    [JsonPropertyName("speed")]
    public decimal Speed { get; set; }
}

/// <summary>
/// Data cakupan awan
/// </summary>
public class CloudInfo
{
    [JsonPropertyName("all")]
    public int All { get; set; }
}

/// <summary>
/// Data sistem (negara)
/// </summary>
public class SysInfo
{
    [JsonPropertyName("country")]
    public string Country { get; set; }
}
