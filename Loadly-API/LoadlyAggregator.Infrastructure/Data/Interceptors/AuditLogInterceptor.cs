using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Attributes;
using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Helpers;
using LoadlyAggregator.Shared.Api;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.Infrastructure.Data.Interceptors;

public class AuditLogInterceptor : SaveChangesInterceptor
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<AuditLogInterceptor> _logger;
    private readonly IHttpContextAccessor? _httpContextAccessor;

    private static readonly JsonSerializerOptions JsonOptions = CreateAuditLogJsonOptions();

    private static JsonSerializerOptions CreateAuditLogJsonOptions()
    {
        var options = new JsonSerializerOptions
        {
            MaxDepth = 64,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
        ApiDateTimeJsonSerializerOptions.ApplyTo(options);
        return options;
    }

    public AuditLogInterceptor(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<AuditLogInterceptor> logger,
        IHttpContextAccessor? httpContextAccessor = null)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    private List<(AuditLog Log, BaseEntity Entity)>? _pendingEntries;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            _pendingEntries = CollectAuditEntries(eventData.Context);
        }
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        await PersistAuditLogsAsync();
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private static readonly ConcurrentDictionary<Type, bool> NoAuditCache = new();

    private static bool HasNoAudit(Type type) =>
        NoAuditCache.GetOrAdd(type, t => t.GetCustomAttribute<NoAuditAttribute>() is not null);

    private List<(AuditLog Log, BaseEntity Entity)>? CollectAuditEntries(DbContext context)
    {
        var entries = context.ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity
                && e.Entity is not AuditLog
                && !HasNoAudit(e.Metadata.ClrType)
                && (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))
            .ToList();

        if (entries.Count == 0) return null;

        var (userId, userType) = GetCurrentUser();
        var (ipAddress, userAgent) = GetRequestInfo();
        var correlationId = GetCorrelationId();
        var pending = new List<(AuditLog Log, BaseEntity Entity)>(entries.Count);

        foreach (var entry in entries)
        {
            if (entry.Entity is not BaseEntity entity) continue;

            try
            {
                pending.Add((BuildAuditLog(entry, entity, userId, userType, ipAddress, userAgent, correlationId), entity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to build audit log for entity {EntityType} with ID {EntityId}",
                    entry.Metadata.ClrType.Name, entity.Id);
            }
        }

        return pending.Count > 0 ? pending : null;
    }

    private async Task PersistAuditLogsAsync()
    {
        var pending = _pendingEntries;
        _pendingEntries = null;

        if (pending is null || pending.Count == 0) return;

        foreach (var (log, entity) in pending)
        {
            log.EntityId = entity.Id;
        }

        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var logsContext = scope.ServiceProvider.GetRequiredService<LogsDbContext>();
            logsContext.AuditLogs.AddRange(pending.Select(p => p.Log));
            await logsContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to persist {Count} audit log entries", pending.Count);
        }
    }

    private static AuditLog BuildAuditLog(
        EntityEntry entry, BaseEntity entity,
        long? userId, string userType,
        string ipAddress, string userAgent,
        string? correlationId)
    {
        var eventType = entry.State switch
        {
            EntityState.Added => AuditEventType.CREATE,
            EntityState.Modified => AuditEventType.UPDATE,
            EntityState.Deleted => AuditEventType.DELETE,
            _ => AuditEventType.CREATE
        };

        var (oldValues, newValues) = BuildValueSnapshots(entry, entry.State);

        return new AuditLog
        {
            EventType = eventType.ToString(),
            EntityType = entry.Metadata.ClrType.Name,
            UserId = userId,
            UserType = userType,
            OldValues = oldValues,
            NewValues = newValues,
            ChangeSummary = BuildChangeSummary(entry, entry.State),
            IpAddress = ipAddress,
            UserAgent = userAgent,
            CorrelationId = correlationId,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static (JsonDocument? OldValues, JsonDocument? NewValues) BuildValueSnapshots(
        EntityEntry entry,
        EntityState state) =>
        state switch
        {
            EntityState.Added => (null, ToDocument(CollectScalarValues(entry))),
            EntityState.Deleted => (ToDocument(CollectScalarValues(entry)), null),
            EntityState.Modified => BuildModifiedSnapshots(entry),
            _ => (null, null)
        };

    private static (JsonDocument? OldValues, JsonDocument? NewValues) BuildModifiedSnapshots(EntityEntry entry)
    {
        var oldValues = new Dictionary<string, object?>();
        var newValues = new Dictionary<string, object?>();

        foreach (var property in entry.Properties.Where(p => p.IsModified))
        {
            if (ShouldSkip(property)) continue;
            oldValues[property.Metadata.Name] = property.OriginalValue;
            newValues[property.Metadata.Name] = property.CurrentValue;
        }

        return oldValues.Count > 0
            ? (ToDocument(oldValues), ToDocument(newValues))
            : (null, null);
    }

    private static Dictionary<string, object?> CollectScalarValues(EntityEntry entry)
    {
        var values = new Dictionary<string, object?>();
        foreach (var property in entry.Properties)
        {
            if (ShouldSkip(property)) continue;
            values[property.Metadata.Name] = property.CurrentValue;
        }

        return values;
    }

    private static JsonDocument? ToDocument(Dictionary<string, object?> values) =>
        values.Count > 0 ? JsonSerializer.SerializeToDocument(values, JsonOptions) : null;

    private static List<string>? BuildChangeSummary(EntityEntry entry, EntityState state)
    {
        IEnumerable<PropertyEntry> properties = state switch
        {
            EntityState.Modified => entry.Properties.Where(p => p.IsModified),
            EntityState.Added or EntityState.Deleted => entry.Properties,
            _ => []
        };

        var names = properties
            .Where(p => !ShouldSkip(p))
            .Select(p => p.Metadata.Name)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToList();

        return names.Count > 0 ? names : null;
    }

    private static readonly HashSet<string> SensitiveProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "Password", "TokenHash", "TokenFamilyId", "ReplacedByTokenHash",
        "FcmToken", "BiometricFingerprint"
    };

    private static bool ShouldSkip(PropertyEntry property) =>
        property.Metadata.IsShadowProperty()
        || SensitiveProperties.Contains(property.Metadata.Name)
        || !property.Metadata.ClrType.IsScalarType();

    private (long? userId, string userType) GetCurrentUser()
    {
        if (_httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated != true)
            return (null, "System");

        var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        var userTypeClaim = _httpContextAccessor.HttpContext.User.FindFirst("userType");

        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
            return (userId, userTypeClaim?.Value ?? "Unknown");

        return (null, "System");
    }

    private (string ipAddress, string userAgent) GetRequestInfo()
    {
        var http = _httpContextAccessor?.HttpContext;
        if (http is null) return ("Unknown", "Unknown");

        var ip = DeviceInfoHelper.GetIpAddress(http);
        var ua = DeviceInfoHelper.GetUserAgent(http);
        return (
            string.IsNullOrWhiteSpace(ip) ? "Unknown" : ip,
            string.IsNullOrWhiteSpace(ua) ? "Unknown" : ua
        );
    }

    private string? GetCorrelationId()
    {
        var http = _httpContextAccessor?.HttpContext;
        if (http is null) return null;

        if (http.Items.TryGetValue(CorrelationIdConstants.HttpContextItemKey, out var item)
            && item is string itemCorrelationId
            && !string.IsNullOrWhiteSpace(itemCorrelationId))
        {
            return TruncateCorrelationId(itemCorrelationId);
        }

        if (http.Request.Headers.TryGetValue(CorrelationIdConstants.HeaderName, out var headerValue)
            && !string.IsNullOrWhiteSpace(headerValue.ToString()))
        {
            return TruncateCorrelationId(headerValue.ToString());
        }

        var traceId = http.TraceIdentifier;
        return string.IsNullOrWhiteSpace(traceId) ? null : TruncateCorrelationId(traceId);
    }

    private static string TruncateCorrelationId(string correlationId) =>
        correlationId.Length <= CorrelationIdConstants.MaxLength
            ? correlationId
            : correlationId[..CorrelationIdConstants.MaxLength];
}

internal static class TypeExtensions
{
    private static readonly HashSet<Type> ScalarTypes = new()
    {
        typeof(string), typeof(decimal), typeof(Guid),
        typeof(DateTime), typeof(DateTimeOffset), typeof(DateOnly), typeof(TimeOnly), typeof(TimeSpan)
    };

    public static bool IsScalarType(this Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;
        return underlying.IsPrimitive || underlying.IsEnum || ScalarTypes.Contains(underlying);
    }
}
