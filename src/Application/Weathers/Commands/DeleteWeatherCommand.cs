public class DeleteWeatherCommand : WeatherDto, IRequest<bool>
{
    public Guid Id { get; }

    public DeleteWeatherCommand(Guid id)
    {
        Id = id;
    }
}
