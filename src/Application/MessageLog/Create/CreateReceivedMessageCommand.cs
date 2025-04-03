// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace NetCa.Application.MessageLog.Create;

/// <summary>
/// CreateReceivedMessageCommand
/// </summary>
public class CreateReceivedMessageCommand : IRequest<Unit>
{
    /// <summary>
    /// Gets or sets Message
    /// </summary>
    public ReceivedMessageBroker Message { get; set; }
}

/// <summary>
/// Handling CreateReceivedMessageCommand
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CreateReceivedMessageCommandHandler"/> class.
/// </remarks>
/// <param name="_context">Set context to perform CRUD into Database</param>
/// <param name="_logger">Set logger to perform logging</param>
public class CreateReceivedMessageCommandHandler(
    IApplicationDbContext _context,
    ILogger<CreateReceivedMessageCommandHandler> _logger
) : IRequestHandler<CreateReceivedMessageCommand, Unit>
{
    /// <summary>
    /// Handle Create Received Message Command
    /// </summary>
    /// <param name="request">
    /// The encapsulated request body
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to perform cancel the operation
    /// </param>
    /// <returns></returns>
    public async Task<Unit> Handle(
        CreateReceivedMessageCommand request,
        CancellationToken cancellationToken)
    {
        _context.ReceivedMessageBroker.Add(request.Message);

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogDebug("Save received message success");

        return Unit.Value;
    }
}
