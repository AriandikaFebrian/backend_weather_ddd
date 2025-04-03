// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace NetCa.Application.MessageLog.Delete;

/// <summary>
/// DeleteSendMessageCommand
/// </summary>
public class DeleteSendMessageCommand : IRequest<bool>
{
}

/// <summary>
/// Handling DeleteSendMessageCommand
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DeleteSendMessageCommandHandler"/> class.
/// </remarks>
/// <param name="_context">Set context to perform CRUD into Database</param>
/// <param name="_logger">Set logger to perform logging</param>
/// <param name="_appSetting">Set dateTime to get Application Setting</param>
public class DeleteSendMessageCommandHandler(
    IApplicationDbContext _context,
    ILogger<DeleteSendMessageCommandHandler> _logger,
    AppSetting _appSetting
) : IRequestHandler<DeleteSendMessageCommand, bool>
{
    /// <summary>
    /// Handle Delete Send Message
    /// </summary>
    /// <param name="request">
    /// The encapsulated request body
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to perform cancel the operation
    /// </param>
    /// <returns></returns>
    public async Task<bool> Handle(
        DeleteSendMessageCommand request,
        CancellationToken cancellationToken)
    {
        var status = false;

        try
        {
            _logger.LogDebug("Delete send message process");

            var lifeTime = _appSetting.DataLifetime.Changelog;

            var date = DateTime.Now.AddDays(-lifeTime);

            await _context
                .MessageBroker
                .Where(x => date > x.StoredDate)
                .DeleteAsync(cancellationToken)
                .ConfigureAwait(false);

            status = true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete received message: {Message}", e.Message);
        }

        _logger.LogDebug("Delete send message done");

        return status;
    }
}
