// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

namespace NetCa.Application.IntegrationTests;

using static Testing;

/// <summary>
/// BaseTestFixture
/// </summary>
[TestFixture]
public abstract class BaseTestFixture
{
    /// <summary>
    /// TestSetUp
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [SetUp]
    public async Task TestSetUp()
    {
        await ResetData().ConfigureAwait(false);
    }
}
