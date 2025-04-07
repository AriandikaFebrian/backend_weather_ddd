// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Reflection;
using NetCa.Application.Common.Vms;
using NetCa.Application.Weathers.Commands.GetWeather;
namespace NetCa.Application.Common.Mappings;

/// <summary>
/// MappingProfile
/// </summary>
public class MappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappingProfile"/> class.
    /// </summary>
    public MappingProfile()
    {
        ApplyMappingsFromAssembly(typeof(DependencyInjection).Assembly);
        CreateMap<CreateWeatherCommand, Weather>();
        CreateMap<Weather, WeatherDto>();
        CreateMap<Weather, WeatherVm>()
            .ForMember(dest => dest.DomainEvents, opt => opt.Ignore())  // Ignore DomainEvents
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.WeatherDescription, opt => opt.MapFrom(src => src.WeatherDescription))
            .ForMember(dest => dest.Temperature, opt => opt.MapFrom(src => src.Temperature))
            .ForMember(dest => dest.Humidity, opt => opt.MapFrom(src => src.Humidity))
            .ForMember(dest => dest.WindSpeed, opt => opt.MapFrom(src => src.WindSpeed))
            .ForMember(dest => dest.CloudCoverage, opt => opt.MapFrom(src => src.CloudCoverage))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp));
    }
    

    /// <summary>
    /// ApplyMappingsFromAssembly
    /// </summary>
    /// <param name="assembly"></param>
    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        var types = assembly
            .GetExportedTypes()
            .Where(
                t =>
                    Array.Exists(
                        t.GetInterfaces(),
                        i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)
                    )
            )
            .ToList();

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);

            var methodInfo =
                type.GetMethod("Mapping") ?? type.GetInterface("IMapFrom`1")?.GetMethod("Mapping");

            methodInfo?.Invoke(instance, [this]);
        }
    }
}
