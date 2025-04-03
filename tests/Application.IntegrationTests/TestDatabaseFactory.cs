// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using NetCa.Application.Common.Models;

namespace NetCa.Application.IntegrationTests;

/// <summary>
/// TestDatabaseFactory
/// </summary>
public static class TestDatabaseFactory
{
    /// <summary>
    /// TestDatabaseFactory
    /// </summary>
    /// <param name="appSetting"/>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task<ITestDatabase> CreateAsync(AppSetting appSetting)
    {
        var database = new PostgresTestDatabase();

        await database.InitializeAsync().ConfigureAwait(false);

        return database;
    }
}
