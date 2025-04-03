using NetCa.Application.Common.Exceptions;
using NetCa.Application.Weathers.Commands.GetWeather;
using NetCa.Domain.Constants;
using NetCa.Domain.Entities;
using NetCa.Infrastructure.Data;
using static NetCa.Application.IntegrationTests.Testing;

namespace NetCa.Application.IntegrationTests.Weathers.Commands;

public class CreateWeatherCommandTest : BaseTestFixture
{
    private readonly ApplicationDbContext _context = Context;

    /// <summary>
    /// ShouldCreateWeather
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldCreateWeather()
    {
        var id = Guid.NewGuid();

        var weather = new Weather
        {
            Id = id,
            City = "Jakarta",
            WeatherDescription = "Sunny day",
            Temperature = 30.5m, // ✅ Decimal
            Humidity = "85%", // ✅ String tetap string
            WindSpeed = 10.2m, // ✅ Decimal
            CloudCoverage = 80, // ✅ Integer
            Country = "Indonesia",
            Timestamp = DateTime.UtcNow, // ✅ Gunakan DateTime.UtcNow
            CreatedBy = SystemConstants.Name,
            CreatedDate = DateTimeOffset.UtcNow, // ✅ Gunakan UtcNow
            UpdatedBy = null,
            UpdatedDate = null,
            IsActive = true
        };

        _context.Weathers.Add(weather);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        var test = await _context.Weathers
            .FirstOrDefaultAsync(x => x.Id.Equals(id)).ConfigureAwait(false);

        test.Should().NotBeNull();
    }

    [Test]
    public async Task ShouldRequiredField()
    {
        var command = new CreateWeatherCommand
        {
            City = "Jakarta",
            WeatherDescription = "Sunny day",
            Temperature = 30.5m, // ✅ Decimal
            Humidity = "85%", // ✅ String tetap string
            WindSpeed = 10.2m, // ✅ Decimal
            CloudCoverage = 80, // ✅ Integer
            Country = "Indonesia",
            Timestamp = DateTime.UtcNow // ✅ Gunakan DateTime.UtcNow
        };

        await FluentActions
            .Invoking(() => SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>()
            .ConfigureAwait(false);
    }
}
