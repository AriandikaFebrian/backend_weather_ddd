// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace NetCa.Application.Changelogs.Commands.DeleteChangelog;

/// <summary>
/// DeleteChangelogCommand
/// </summary>
public class DeleteChangelogCommand : IRequest<bool>
{
    /// <summary>
    /// Handling DeleteChangelogCommand
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="DeleteChangelogCommandHandler"/> class.
    /// </remarks>
    /// <param name="_context">Set context to perform CRUD into Database</param>
    /// <param name="_logger">Set logger to perform logging</param>
    /// <param name="_appSetting">Set dateTime to get Application Setting</param>
    /// <returns></returns>
    public class DeleteChangelogCommandHandler(
        IApplicationDbContext _context,
        ILogger<DeleteChangelogCommandHandler> _logger,
        AppSetting _appSetting
    ) : IRequestHandler<DeleteChangelogCommand, bool>
    {
        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="request">
        /// The encapsulated request body
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token to perform cancel the operation
        /// </param>
        /// <returns>A bool true or false</returns>
        public async Task<bool> Handle(
            DeleteChangelogCommand request,
            CancellationToken cancellationToken)
        {
            var status = false;

            try
            {
                var lifeTime = _appSetting.DataLifetime.Changelog;

                var date = DateTime.Now.AddDays(-lifeTime);

                await _context
                    .Changelogs
                    .Where(x => date > x.ChangeDate)
                    .DeleteAsync(cancellationToken)
                    .ConfigureAwait(false);

                status = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to delete changelog: {Message}", e.Message);
            }

            return status;
        }
    }
}
