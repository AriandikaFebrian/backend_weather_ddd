// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using NetCa.Application.Changelogs.Commands.DeleteChangelog;
using NetCa.Domain.Entities;
using NetCa.Infrastructure.Data;
using static NetCa.Application.IntegrationTests.Testing;

namespace NetCa.Application.IntegrationTests.Changelogs.Commands;

/// <summary>
/// DeleteChangelogCommandTest
/// </summary>
public class DeleteChangelogCommandTest : BaseTestFixture
{
    private readonly ApplicationDbContext _context = Context;

    /// <summary>
    /// ShouldDeleteChangelog
    /// </summary>
    /// <param name="changeDate"/>
    /// <param name="shouldDelete"/>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestCase("2000-01-01", true)]
    [TestCase("2100-01-01", false)]
    public async Task ShouldDeleteChangelog(DateTime changeDate, bool shouldDelete)
    {
        var query = new DeleteChangelogCommand();

        var id = Guid.NewGuid();

        _context.Changelogs.Add(new Changelog { Id = id, ChangeDate = changeDate });

        await _context.SaveChangesAsync().ConfigureAwait(false);

        var test = await _context
            .Changelogs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Equals(id))
            .ConfigureAwait(false);

        test.Should().NotBeNull();

        (await SendAsync(query).ConfigureAwait(false)).Should().BeTrue();

        test = await _context
            .Changelogs
            .FirstOrDefaultAsync(x => x.Id.Equals(id))
            .ConfigureAwait(false);

        if (shouldDelete)
        {
            test.Should().BeNull();
        }
        else
        {
            test.Should().NotBeNull();
        }
    }
}
