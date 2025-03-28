// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace NetCa.Application.MessageLog.Delete;

/// <summary>
/// DeleteReceivedMessageCommand
/// </summary>
public class DeleteReceivedMessageCommand : IRequest<bool>
{
}

/// <summary>
/// Handling DeleteReceivedMessageCommand
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DeleteReceivedMessageCommandHandler"/> class.
/// </remarks>
/// <param name="_context">Set context to perform CRUD into Database</param>
/// <param name="_logger">Set logger to perform logging</param>
/// <param name="_appSetting">Set dateTime to get Application Setting</param>
public class DeleteReceivedMessageCommandHandler(
    IApplicationDbContext _context,
    ILogger<DeleteReceivedMessageCommandHandler> _logger,
    AppSetting _appSetting
    )
        : IRequestHandler<DeleteReceivedMessageCommand, bool>
{
    /// <summary>
    /// Handle Delete Received Message
    /// </summary>
    /// <param name="request">
    /// The encapsulated request body
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to perform cancel the operation
    /// </param>
    /// <returns></returns>
    public async Task<bool> Handle(
        DeleteReceivedMessageCommand request,
        CancellationToken cancellationToken)
    {
        var status = false;

        try
        {
            _logger.LogDebug("Delete received message process");

            var lifeTime = _appSetting.DataLifetime.Changelog;

            var date = DateTime.Now.AddDays(-lifeTime);

            await _context
                .ReceivedMessageBroker
                .Where(x => date > x.TimeIn)
                .DeleteAsync(cancellationToken)
                .ConfigureAwait(false);

            status = true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete received message: {Message}", e.Message);
        }

        _logger.LogDebug("Delete received message done");

        return status;
    }
}
