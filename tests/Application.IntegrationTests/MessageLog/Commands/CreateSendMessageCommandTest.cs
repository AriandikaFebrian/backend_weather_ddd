// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using NetCa.Application.MessageLog.Create;
using NetCa.Domain.Entities;
using NetCa.Infrastructure.Data;
using static NetCa.Application.IntegrationTests.Testing;

namespace NetCa.Application.IntegrationTests.MessageLog.Commands;

/// <summary>
/// CreateSendMessageCommandTest
/// </summary>
public class CreateSendMessageCommandTest : BaseTestFixture
{
    private readonly ApplicationDbContext _context = Context;

    /// <summary>
    /// ShouldCreateSendMessage
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ShouldCreateSendMessage()
    {
        var id = Guid.NewGuid();

        var query = new CreateSendMessageCommand { MessageBroker = new MessageBroker { Id = id } };

        await SendAsync(query).ConfigureAwait(false);

        var test = await _context.MessageBroker
            .FirstOrDefaultAsync(x => x.Id.Equals(id)).ConfigureAwait(false);

        test.Should().NotBeNull();
    }
}
