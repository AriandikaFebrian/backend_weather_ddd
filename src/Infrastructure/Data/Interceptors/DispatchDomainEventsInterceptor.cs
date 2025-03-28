// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NetCa.Domain.Common;

namespace NetCa.Infrastructure.Data.Interceptors;

/// <summary>
/// DispatchDomainEventsInterceptor
/// </summary>
/// <param name="_mediator"/>
public class DispatchDomainEventsInterceptor(IMediator _mediator) : SaveChangesInterceptor
{
    /// <summary>
    /// SavingChanges
    /// </summary>
    /// <param name="eventData"/>
    /// <param name="result"/>
    /// <returns></returns>
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult();

        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// SavingChanges
    /// </summary>
    /// <param name="eventData"/>
    /// <param name="result"/>
    /// <param name="cancellationToken"/>
    /// <returns></returns>
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvents(eventData.Context).ConfigureAwait(false);

        return await base.SavingChangesAsync(eventData, result, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// SavingChanges
    /// </summary>
    /// <param name="context"/>
    /// <returns></returns>
    public async Task DispatchDomainEvents(DbContext context)
    {
        if (context == null)
        {
            return;
        }

        var entities = context
            .ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity);

        var domainEvents = entities.SelectMany(e => e.DomainEvents).ToList();

        entities.ToList().ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent).ConfigureAwait(false);
        }
    }
}
