using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Infrastructure.Data.Interceptors;

/// <summary>
/// Centralizes DateTime kind normalization and IAuditable timestamp stamping
/// as a SaveChangesInterceptor, so it applies to all DbContexts via DI.
///
/// NOTE: This interceptor only fires for code paths that go through SaveChanges/SaveChangesAsync.
/// Raw SQL operations (ExecuteUpdateAsync, ExecuteDeleteAsync, FlexLabs Upsert, etc.)
/// bypass the ChangeTracker and must set UpdatedAt manually.
/// </summary>
public class AuditableTimestampInterceptor : SaveChangesInterceptor
{
    private static readonly ConcurrentDictionary<IEntityType, IReadOnlyList<IProperty>> DateTimePropertiesCache = new();

    private readonly TimeProvider _timeProvider;

    public AuditableTimestampInterceptor(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            ApplyPipeline(eventData.Context);
        }
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            ApplyPipeline(eventData.Context);
        }
        return base.SavingChanges(eventData, result);
    }

    private void ApplyPipeline(DbContext context)
    {
        NormalizeDateTimeValues(context);
        ApplyAuditableTimestamps(context);
    }

    private static void NormalizeDateTimeValues(DbContext context)
    {
        List<EntityEntry> entries = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .ToList();

        foreach (EntityEntry entry in entries)
        {
            var dateTimeProperties = DateTimePropertiesCache.GetOrAdd(
                entry.Metadata,
                entityType => entityType.GetProperties()
                    .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?))
                    .ToList());

            foreach (IProperty prop in dateTimeProperties)
            {
                PropertyEntry property = entry.Property(prop.Name);

                if (property.CurrentValue is DateTime dateTimeValue)
                {
                    if (dateTimeValue.Kind == DateTimeKind.Unspecified)
                    {
                        property.CurrentValue = DateTime.SpecifyKind(dateTimeValue, DateTimeKind.Utc);
                    }
                    else if (dateTimeValue.Kind == DateTimeKind.Local)
                    {
                        property.CurrentValue = dateTimeValue.ToUniversalTime();
                    }
                }
            }
        }
    }

    private void ApplyAuditableTimestamps(DbContext context)
    {
        DateTime now = _timeProvider.GetUtcNow().UtcDateTime;

        foreach (EntityEntry entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is not IAuditable auditable)
                continue;

            if (entry.State == EntityState.Added)
            {
                auditable.CreatedAt = now;
                auditable.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(IAuditable.CreatedAt)).IsModified = false;
                auditable.UpdatedAt = now;
            }
        }
    }
}
