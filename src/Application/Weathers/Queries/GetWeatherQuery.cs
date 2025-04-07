using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NetCa.Application.Common.Dtos;
using NetCa.Domain.Entities;
using MediatR;

namespace NetCa.Application.Weathers.Queries
{
    /// <summary>
    /// GetWeatherQuery
    /// </summary>
    public record GetWeatherQuery(string City) : IRequest<List<WeatherDto>>;
    public record GetWeatherByIdQuery(Guid Id) : IRequest<WeatherDto>;
    public record GetAllWeatherQuery(int PageNumber = 1, int PageSize = 50) : IRequest<List<WeatherDto>>;

    /// <summary>
    /// GetWeatherQueryHandler
    /// </summary>
    public class GetWeatherQueryHandler : IRequestHandler<GetWeatherQuery, List<WeatherDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "1201e499176a67bbf3e65350e41cf231";

        public GetWeatherQueryHandler(IApplicationDbContext context, IMapper mapper, HttpClient httpClient)
        {
            _context = context;
            _mapper = mapper;
            _httpClient = httpClient;
        }

        public async Task<List<WeatherDto>> Handle(GetWeatherQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.City))
            {
                throw new ArgumentException("City parameter is required.");
            }

            string cityNormalized = request.City?.Trim().ToLowerInvariant() ?? string.Empty;

            var url = $"https://api.openweathermap.org/data/2.5/weather?q={request.City}&appid={_apiKey}&units=metric";

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Failed to fetch weather data: {ex.Message}");
            }

            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var openWeatherData = JsonSerializer.Deserialize<OpenWeatherMapResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (openWeatherData == null)
            {
                throw new Exception("Failed to parse weather data.");
            }

            var weatherEntity = new Weather
            {
                City = openWeatherData.Name,
                WeatherDescription = openWeatherData.Weather[0].Description,
                Temperature = openWeatherData.Main.Temp,
                Humidity = $"{openWeatherData.Main.Humidity}%",
                WindSpeed = openWeatherData.Wind.Speed,
                CloudCoverage = openWeatherData.Clouds.All,
                Country = openWeatherData.Sys.Country,
                Timestamp = DateTime.UtcNow,
                Id = Guid.NewGuid() // Ensure unique ID for each record
            };

            // Add the new weather data to the database (do not check if it exists)
            _context.Weathers.Add(weatherEntity);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // Map the Weather entity to WeatherDto for the response
            var WeatherDto = _mapper.Map<WeatherDto>(weatherEntity);

            return new List<WeatherDto> { WeatherDto };
        }
    }

    /// <summary>
    /// DeleteWeatherCommandHandler
    /// </summary>
    public class DeleteWeatherCommandHandler : IRequestHandler<DeleteWeatherCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteWeatherCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteWeatherCommand request, CancellationToken cancellationToken)
        {
            // Find the weather record by Id
            var weather = await _context.Weathers.FindAsync(new object[] { request.Id }, cancellationToken);

            if (weather == null)
            {
                return false;
            }

            weather.IsDeleted = true;
            weather.IsActive = false;

            // Update the record in the DbContext (no need to remove it)
            _context.Weathers.Update(weather);

            // Save changes to the database
            await _context.SaveChangesAsync(cancellationToken);

            return true; // Successfully marked as deleted (soft delete)
        }
    }

    /// <summary>
    /// GetAllWeatherQueryHandler
    /// </summary>
    public class GetAllWeatherQueryHandler : IRequestHandler<GetAllWeatherQuery, List<WeatherDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllWeatherQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<WeatherDto>> Handle(GetAllWeatherQuery request, CancellationToken cancellationToken)
    {
        var skip = (request.PageNumber - 1) * request.PageSize;

        var weatherList = await _context.Weathers
            .Where(w => w.IsDeleted == false || w.IsDeleted == null)
            .OrderByDescending(w => w.Timestamp) // optional: urutkan dari terbaru
            .Skip(skip)
            .Take(request.PageSize)
            .ProjectTo<WeatherDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return weatherList;
    }
}
    /// <summary>
    /// GetWeatherByIdQueryHandler
    /// </summary>
    public class GetWeatherByIdQueryHandler : IRequestHandler<GetWeatherByIdQuery, WeatherDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetWeatherByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<WeatherDto> Handle(GetWeatherByIdQuery request, CancellationToken cancellationToken)
        {
            var weather = await _context.Weathers
                .Where(w => w.Id == request.Id && (w.IsDeleted == false || w.IsDeleted == null))
                .FirstOrDefaultAsync(cancellationToken);

            return _mapper.Map<WeatherDto>(weather);
        }
    }

    public class UpdateWeatherCommandHandler : IRequestHandler<UpdateWeatherCommand, WeatherDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UpdateWeatherCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<WeatherDto> Handle(UpdateWeatherCommand request, CancellationToken cancellationToken)
        {
            var weather = await _context.Weathers.FindAsync(request.Id);
            if (weather == null)
            {
                throw new KeyNotFoundException("Weather record not found.");
            }

            weather.City = request.City;
            weather.WeatherDescription = request.WeatherDescription;
            weather.Temperature = request.Temperature;
            weather.Humidity = request.Humidity;
            weather.WindSpeed = request.WindSpeed;
            weather.CloudCoverage = request.CloudCoverage;
            weather.Country = request.Country;
            weather.Timestamp = request.Timestamp;

            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<WeatherDto>(weather);
        }
    }
}
