namespace NetCa.Application.Weathers.Commands.GetWeather;

/// <summary>
/// Command to create a new weather record.
/// </summary>
public class CreateWeatherCommand : WeatherDto, IRequest<Unit> { }

/// <summary>
/// Handles the creation of a new weather record by mapping the request and saving it to the database.
/// </summary>
public class CreateWeatherCommandHandler : IRequestHandler<CreateWeatherCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateWeatherCommandHandler"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="mapper">The AutoMapper instance for mapping the command to an entity.</param>
    public CreateWeatherCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the creation of the weather record.
    /// </summary>
    /// <param name="request">The weather creation command.</param>
    /// <param name="cancellationToken">The cancellation token to observe while saving the data.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the unit value.</returns>
    public async Task<Unit> Handle(CreateWeatherCommand request, CancellationToken cancellationToken)
    {
        var weather = _mapper.Map<Weather>(request);

        _context.Weathers.Add(weather);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Unit.Value;
 }
}

/// <summary>
/// Validator for the <see cref="CreateWeatherCommand"/> to ensure valid data before processing.
/// </summary>
public class CreateWeatherCommandValidator : AbstractValidator<CreateWeatherCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateWeatherCommandValidator"/> class.
    /// </summary>
    public CreateWeatherCommandValidator()
    {
        RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .Length(3, 100).WithMessage("City name must be between 3 and 100 characters.");

        RuleFor(x => x.Temperature)
                .GreaterThanOrEqualTo(-100).WithMessage("Temperature must be greater than or equal to -100°C.")
                .LessThanOrEqualTo(60).WithMessage("Temperature must be less than or equal to 60°C.");
    }
}
