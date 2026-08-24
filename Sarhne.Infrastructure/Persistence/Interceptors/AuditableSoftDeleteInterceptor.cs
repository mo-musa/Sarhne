using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Sarhne.Application.Contracts.Services.Authentication.CurrentUser;
using Sarhne.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Infrastructure.Persistence.Interceptors;

public sealed class AuditableSoftDeleteInterceptor
    : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    public AuditableSoftDeleteInterceptor(
        ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyChanges(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyChanges(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyChanges(DbContext? context)
    {
        if (context is null)
            return;

        var entries = context.ChangeTracker
        .Entries()
        .Where(entry =>
            entry.State is EntityState.Added
                or EntityState.Modified
                or EntityState.Deleted)
        .ToList();

        var currentUserId = _currentUserService.UserId;

        foreach (var entry in entries)
        {
            var wasDeleted = entry.State == EntityState.Deleted;

            if (entry.Entity is ISoftDeletableEntity softDeletableEntity)
            {
                if (wasDeleted)
                {
                    softDeletableEntity.SoftDelete(currentUserId);
                    entry.State = EntityState.Modified;
                }
            }

            if (entry.Entity is IAuditableEntity auditableEntity)
            {
                HandleAudit(
                    entry,
                    auditableEntity,
                    currentUserId,
                    wasDeleted);
            }
        }
    }

    private static void HandleSoftDelete(
        EntityEntry entry,
        ISoftDeletableEntity entity,
        int? currentUserId)
    {
        if (entry.State == EntityState.Deleted)
        {
            entity.SoftDelete(currentUserId);

            //entity.DeletedById = currentUserId;

            entry.State = EntityState.Modified;
        }
    }

    private static void HandleAudit(
    EntityEntry entry,
    IAuditableEntity entity,
    int? currentUserId,
    bool wasDeleted)
    {
        if (wasDeleted)
            return;

        switch (entry.State)
        {
            case EntityState.Added:

                entity.CreatedAt = DateTime.UtcNow;
                entity.CreatedById = currentUserId;

                break;

            case EntityState.Modified:

                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedById = currentUserId;

                entry.Property(nameof(IAuditableEntity.CreatedAt))
                    .IsModified = false;

                entry.Property(nameof(IAuditableEntity.CreatedById))
                    .IsModified = false;

                break;
        }
    }
}
