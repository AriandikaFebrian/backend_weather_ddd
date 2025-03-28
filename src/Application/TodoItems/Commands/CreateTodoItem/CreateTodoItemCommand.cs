// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Globalization;

namespace NetCa.Application.TodoItems.Commands.CreateTodoItem;

/// <summary>
/// CreateTodoItemCommand
/// </summary>
public class CreateTodoItemCommand : TodoItemDto, IRequest<Unit> { }

/// <summary>
/// CreateTodoItemCommandHandler
/// </summary>
public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTodoItemCommandHandler"/> class.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="mapper"></param>
    public CreateTodoItemCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Handle
    /// </summary>
    /// /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Unit> Handle(
        CreateTodoItemCommand request,
        CancellationToken cancellationToken
    )
    {
        await _context
            .ExecuteResiliencyAsync(() => HandleProcess(request, cancellationToken))
            .ConfigureAwait(false);

        return Unit.Value;
    }

    private async Task HandleProcess(
        CreateTodoItemCommand request,
        CancellationToken cancellationToken
    )
    {
        var todoItem = _mapper.Map<TodoItem>(request);

        _context.TodoItems.Add(todoItem);

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}

/// <summary>
/// CreateTodoItemCommandValidator
/// </summary>
public class CreateTodoItemCommandValidator : AbstractValidator<CreateTodoItemCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTodoItemCommandValidator"/> class.
    /// </summary>
    public CreateTodoItemCommandValidator()
    {
        RuleFor(x => x.Title).MaximumLength(100);

        RuleFor(x => x.Description).MaximumLength(255);

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .Must(IsValidDate)
            .WithMessage("Value of {PropertyName} must format DateOnly. e.g. '2023-12-12'");
    }

    private static bool IsValidDate(string args)
    {
        if (string.IsNullOrWhiteSpace(args))
            return false;

        return DateOnly.TryParse(args, new DateTimeFormatInfo(), out _);
    }
}
