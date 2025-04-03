// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using NetCa.Application.MessageLog.Create;
using NetCa.Domain.Entities;
using static NetCa.Application.IntegrationTests.Testing;

namespace NetCa.Application.IntegrationTests.MessageLog.Commands;

/// <summary>
/// CreateReceivedMessageCommandTest
/// </summary>
public class CreateReceivedMessageCommandTest : BaseTestFixture
{
    /// <summary>
    /// ShouldCreateReceivedMessage
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ShouldCreateReceivedMessage()
    {
        var id = Guid.NewGuid();

        var query = new CreateReceivedMessageCommand
        {
            Message = new ReceivedMessageBroker { Id = id }
        };

        await SendAsync(query).ConfigureAwait(false);

        var test = await Context.ReceivedMessageBroker
            .FirstOrDefaultAsync(x => x.Id.Equals(id)).ConfigureAwait(false);

        test.Should().NotBeNull();
    }
}
