// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Infrastructure;

namespace NetCa.Application.Common.Interfaces;

/// <summary>
/// IApplicationDbContext
/// </summary>
public interface IApplicationDbContext
{
#pragma warning disable
    public DbSet<Changelog> Changelogs { get; set; }
    public DbSet<MessageBroker> MessageBroker { get; set; }
    public DbSet<ReceivedMessageBroker> ReceivedMessageBroker { get; set; }
    public DbSet<TodoItem> TodoItems { get; set; }
    public DbSet<Weather> Weathers { get; set; }
    public DbSet<Tiket> Tikets { get; set; }
#pragma warning restore

    /// <summary>
    /// Gets database
    /// </summary>
    public DatabaseFacade Database { get; }

    /// <summary>
    /// AsNoTracking
    /// </summary>
    public void AsNoTracking();

    /// <summary>
    /// Clear
    /// </summary>
    public void Clear();

    /// <summary>
    /// Execute using EF Core resiliency strategy
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public Task ExecuteResiliencyAsync(Func<Task> action);

    /// <summary>
    /// SaveChangesAsync
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
