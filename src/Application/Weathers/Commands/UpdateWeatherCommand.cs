using NetCa.Application.Common.Dtos;
public record UpdateWeatherCommand(
Guid Id,
string City,
string WeatherDescription,
decimal Temperature,
string Humidity,
decimal WindSpeed,
int CloudCoverage,
string Country,
DateTime Timestamp) : IRequest<WeatherDto>;
