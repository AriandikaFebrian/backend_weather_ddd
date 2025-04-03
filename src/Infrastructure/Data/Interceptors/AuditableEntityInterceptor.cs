// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NetCa.Application.Common.Interfaces;
using NetCa.Domain.Common;

namespace NetCa.Infrastructure.Data.Interceptors;

/// <summary>
/// AuditableEntityInterceptor
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AuditableEntityInterceptor"/> class.
/// </remarks>
/// <param name="_currentUserService"></param>
/// <param name="_dateTime"></param>
public class AuditableEntityInterceptor(
    IUserAuthorizationService _currentUserService,
    TimeProvider _dateTime
) : SaveChangesInterceptor
{
    /// <summary>
    /// SavingChanges
    /// </summary>
    /// <param name="eventData"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// SavingChangesAsync
    /// </summary>
    /// <param name="eventData"></param>
    /// <param name="result"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// UpdateEntities
    /// </summary>
    /// <param name="context"></param>
    private void UpdateEntities(DbContext context)
    {
        if (context == null)
        {
            return;
        }

        var username = _currentUserService.GetUserNameSystem();

        foreach (var entry in context.ChangeTracker.Entries<IBaseAuditableEntity>())
        {
            var entity = entry.Entity;

            switch (entry.State)
            {
                case EntityState.Added:
                    if (entity is BaseAuditableEntity entityBase)
                    {
                        entityBase.Id =
                            entityBase.Id == Guid.Empty ? Guid.NewGuid() : entityBase.Id;
                    }

                    entity.CreatedBy ??= username;
                    entity.CreatedDate ??= _dateTime.GetUtcNow();
                    entity.IsActive ??= true;
                    break;
                case EntityState.Modified:
                    entity.CreatedBy ??= username;
                    entity.CreatedDate ??= _dateTime.GetUtcNow();
                    entity.UpdatedBy = username;
                    entity.UpdatedDate = _dateTime.GetUtcNow();
                    break;
            }
        }
    }
}

/// <summary>
/// Extensions
/// </summary>
public static class Extensions
{
    /// <summary>
    /// HasChangedOwnedEntities
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null
            && r.TargetEntry.Metadata.IsOwned()
            && r.TargetEntry.State is EntityState.Added or EntityState.Modified
        );
}
