using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Extensions;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Api;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.DTOs.Queue;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Infrastructure.Services.Queue;

public class BroadcastNotificationQueueProcessor : IQueueProcessor
{
    private readonly ILogger<BroadcastNotificationQueueProcessor> _logger;
    private readonly IBroadcastNotificationTargetResolver _targetResolver;
    private readonly INotificationTransport _notificationTransport;
    private readonly INotificationRepository _notificationRepository;
    private readonly IQueueService _queueService;
    private readonly IQueueNameResolver _queueNameResolver;
    private readonly int _configuredBatchSize;

    public BroadcastNotificationQueueProcessor(
        ILogger<BroadcastNotificationQueueProcessor> logger,
        IBroadcastNotificationTargetResolver targetResolver,
        INotificationTransport notificationTransport,
        INotificationRepository notificationRepository,
        IQueueService queueService,
        IQueueNameResolver queueNameResolver,
        IOptions<QueueOptions> queueOptions)
    {
        _logger = logger;
        _targetResolver = targetResolver;
        _notificationTransport = notificationTransport;
        _notificationRepository = notificationRepository;
        _queueService = queueService;
        _queueNameResolver = queueNameResolver;
        int configured = queueOptions.Value.BroadcastNotificationBatchSize;
        _configuredBatchSize = configured > 0 ? configured : QueueProcessingDefaults.BroadcastNotificationBatchSize;
    }

    public async Task ProcessMessageAsync(string messageJson, string queueName, CancellationToken cancellationToken = default)
    {
        try
        {
            BroadcastNotificationQueueMessage? message = JsonSerializer.Deserialize<BroadcastNotificationQueueMessage>(
                messageJson,
                ApiDateTimeJsonSerializerOptions.SharedQueueAndRedis);

            _ = message ?? throw new InvalidOperationException($"Failed to deserialize broadcast notification message from queue {queueName}");

            int batchSize = message.BatchSize > 0 ? message.BatchSize : _configuredBatchSize;

            switch (message.Channel)
            {
                case NotificationChannel.PUSH:
                    await ProcessInAppChunkAsync(message, queueName, batchSize, cancellationToken);
                    break;
                case NotificationChannel.EMAIL:
                    await ProcessEmailChunkAsync(message, queueName, batchSize, cancellationToken);
                    break;
                case NotificationChannel.SMS:
                    await ProcessSmsChunkAsync(message, queueName, batchSize, cancellationToken);
                    break;
                default:
                    _logger.LogWarning(
                        "Broadcast notification channel {Channel} is not handled by the queue processor (BroadcastNotificationId {BroadcastNotificationId}).",
                        message.Channel,
                        message.BroadcastNotificationId);
                    break;
            }
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Failed to deserialize broadcast notification message from queue {QueueName}", queueName);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse broadcast notification message JSON from queue {QueueName}", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing broadcast notification message from queue {QueueName}", queueName);
        }
    }

    private async Task ProcessInAppChunkAsync(
        BroadcastNotificationQueueMessage message,
        string queueName,
        int batchSize,
        CancellationToken cancellationToken)
    {
        string targetRole = message.ModelType ?? string.Empty;
        bool hasExplicitIds = message.ModelIds is { Count: > 0 };

        _logger.LogInformation(
            "Processing broadcast in-app chunk. ModelType: {ModelType}, ExplicitIds: {HasIds}, Skip: {Skip}, BatchSize: {BatchSize}, BroadcastNotificationId: {BroadcastNotificationId}",
            targetRole,
            hasExplicitIds,
            message.Skip,
            batchSize,
            message.BroadcastNotificationId);

        IReadOnlyList<long> userIds = hasExplicitIds
            ? message.ModelIds!.Skip(message.Skip).Take(batchSize).ToList()
            : await _targetResolver.GetUserIdsPageAsync(targetRole, message.Skip, batchSize);

        string userType = targetRole.ToLowerInvariant();
        string modelType = NotificationPreferenceCatalog.NormalizeModelType(targetRole);
        NotificationDomain domain = userType.ToNotificationDomain();
        int sentCount = 0;
        int failedCount = 0;
        string title = message.Subject ?? string.Empty;

        var inAppNotifications = new List<LoadlyAggregator.Domain.Common.Entities.Notification>();

        foreach (long userId in userIds)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                await _notificationTransport.SendPushToUserAsync(
                    userId,
                    modelType,
                    title,
                    message.Body,
                    message.Data,
                    message.ClickAction,
                    domain,
                    eventType: null,
                    broadcastNotificationId: message.BroadcastNotificationId);

                inAppNotifications.Add(new LoadlyAggregator.Domain.Common.Entities.Notification
                {
                    ModelId = userId,
                    ModelType = modelType,
                    Title = title,
                    Body = message.Body,
                    EventType = null,
                    ResourceType = null,
                    ResourceId = null,
                    ClickAction = message.ClickAction,
                    BroadcastNotificationId = message.BroadcastNotificationId,
                });

                sentCount++;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send broadcast push to user {UserId} ({UserType})", userId, userType);
                failedCount++;
            }
        }

        if (inAppNotifications.Count > 0)
        {
            try
            {
                await _notificationRepository.AddRangeAsync(inAppNotifications);
                await _notificationRepository.SaveChangesAsync();
                _logger.LogInformation(
                    "Stored {Count} in-app notifications for broadcast {BroadcastNotificationId}",
                    inAppNotifications.Count,
                    message.BroadcastNotificationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Failed to store in-app notifications for broadcast {BroadcastNotificationId}",
                    message.BroadcastNotificationId);
            }
        }

        _logger.LogInformation(
            "Broadcast in-app chunk done. ModelType: {ModelType}, Skip: {Skip}, ChunkSize: {Chunk}, Sent: {SentCount}, Failed: {FailedCount}, BroadcastNotificationId: {BroadcastNotificationId}",
            targetRole,
            message.Skip,
            userIds.Count,
            sentCount,
            failedCount,
            message.BroadcastNotificationId);

        if (userIds.Count == batchSize)
        {
            var next = new BroadcastNotificationQueueMessage
            {
                Channel = message.Channel,
                ModelType = message.ModelType,
                ModelIds = message.ModelIds,
                Subject = message.Subject,
                Body = message.Body,
                Data = message.Data,
                ClickAction = message.ClickAction,
                BroadcastNotificationId = message.BroadcastNotificationId,
                AttachmentLandingUrl = message.AttachmentLandingUrl,
                Skip = message.Skip + batchSize,
                BatchSize = batchSize
            };
            await _queueService.EnqueueAsync(queueName, next);
        }
    }

    private async Task ProcessEmailChunkAsync(
        BroadcastNotificationQueueMessage message,
        string queueName,
        int batchSize,
        CancellationToken cancellationToken)
    {
        string targetRole = message.ModelType ?? string.Empty;
        bool hasExplicitIds = message.ModelIds is { Count: > 0 };

        _logger.LogInformation(
            "Processing broadcast email chunk. ModelType: {ModelType}, ExplicitIds: {HasIds}, Skip: {Skip}, BatchSize: {BatchSize}, BroadcastNotificationId: {BroadcastNotificationId}",
            targetRole,
            hasExplicitIds,
            message.Skip,
            batchSize,
            message.BroadcastNotificationId);

        IReadOnlyList<BroadcastContact> contacts;
        if (hasExplicitIds)
        {
            var idsForThisChunk = message.ModelIds!.Skip(message.Skip).Take(batchSize).ToList();
            contacts = await _targetResolver.GetUserContactsByIdsAsync(targetRole, idsForThisChunk);
        }
        else
        {
            contacts = await _targetResolver.GetUserContactsPageAsync(targetRole, message.Skip, batchSize);
        }

        string userType = targetRole.ToLowerInvariant();
        string modelType = NotificationPreferenceCatalog.NormalizeModelType(targetRole);
        NotificationDomain domain = userType.ToNotificationDomain();
        string domainStr = domain.ToStringValue();
        string targetQueueName = _queueNameResolver.Resolve(domainStr);

        int sentCount = 0;
        int skippedCount = 0;

        foreach (var contact in contacts)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(contact.Email))
            {
                _logger.LogWarning(
                    "Skipping broadcast email for user {UserId} ({ModelType}) - no email address",
                    contact.ModelId,
                    modelType);
                skippedCount++;
                continue;
            }

            try
            {
                var emailMessage = new EmailQueueMessage
                {
                    Domain = domainStr,
                    From = null,
                    FromName = null,
                    To = contact.Email,
                    ToName = null,
                    Subject = message.Subject ?? string.Empty,
                    Body = message.Body,
                    IsHtml = true,
                    ModelId = contact.ModelId,
                    ModelType = modelType,
                    EventType = null,
                    BroadcastNotificationId = message.BroadcastNotificationId
                };

                await _queueService.EnqueueAsync(targetQueueName, emailMessage);
                sentCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Failed to enqueue broadcast email for user {UserId} ({ModelType})",
                    contact.ModelId,
                    modelType);
            }
        }

        _logger.LogInformation(
            "Broadcast email chunk done. ModelType: {ModelType}, Skip: {Skip}, ChunkSize: {Chunk}, Queued: {SentCount}, Skipped: {SkippedCount}, BroadcastNotificationId: {BroadcastNotificationId}",
            targetRole,
            message.Skip,
            contacts.Count,
            sentCount,
            skippedCount,
            message.BroadcastNotificationId);

        if (contacts.Count == batchSize)
        {
            var next = new BroadcastNotificationQueueMessage
            {
                Channel = message.Channel,
                ModelType = message.ModelType,
                ModelIds = message.ModelIds,
                Subject = message.Subject,
                Body = message.Body,
                Data = message.Data,
                ClickAction = message.ClickAction,
                BroadcastNotificationId = message.BroadcastNotificationId,
                AttachmentLandingUrl = message.AttachmentLandingUrl,
                Skip = message.Skip + batchSize,
                BatchSize = batchSize
            };
            await _queueService.EnqueueAsync(queueName, next);
        }
    }

    private async Task ProcessSmsChunkAsync(
        BroadcastNotificationQueueMessage message,
        string queueName,
        int batchSize,
        CancellationToken cancellationToken)
    {
        string targetRole = message.ModelType ?? string.Empty;
        bool hasExplicitIds = message.ModelIds is { Count: > 0 };

        _logger.LogInformation(
            "Processing broadcast SMS chunk. ModelType: {ModelType}, ExplicitIds: {HasIds}, Skip: {Skip}, BatchSize: {BatchSize}, BroadcastNotificationId: {BroadcastNotificationId}",
            targetRole,
            hasExplicitIds,
            message.Skip,
            batchSize,
            message.BroadcastNotificationId);

        IReadOnlyList<BroadcastContact> contacts;
        if (hasExplicitIds)
        {
            var idsForThisChunk = message.ModelIds!.Skip(message.Skip).Take(batchSize).ToList();
            contacts = await _targetResolver.GetUserContactsByIdsAsync(targetRole, idsForThisChunk);
        }
        else
        {
            contacts = await _targetResolver.GetUserContactsPageAsync(targetRole, message.Skip, batchSize);
        }

        string userType = targetRole.ToLowerInvariant();
        string modelType = NotificationPreferenceCatalog.NormalizeModelType(targetRole);
        NotificationDomain domain = userType.ToNotificationDomain();
        string domainStr = domain.ToStringValue();
        string targetQueueName = _queueNameResolver.Resolve(domainStr);

        int sentCount = 0;
        int skippedCount = 0;

        foreach (var contact in contacts)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(contact.PhoneNumber))
            {
                _logger.LogWarning(
                    "Skipping broadcast SMS for user {UserId} ({ModelType}) - no phone number",
                    contact.ModelId,
                    modelType);
                skippedCount++;
                continue;
            }

            try
            {
                var smsMessage = new SmsQueueMessage
                {
                    Domain = domainStr,
                    PhoneNumber = contact.PhoneNumber,
                    Message = string.IsNullOrWhiteSpace(message.AttachmentLandingUrl)
                        ? message.Body
                        : $"{message.Body}\n\n{message.AttachmentLandingUrl}",
                    ModelId = contact.ModelId,
                    ModelType = modelType,
                    EventType = null,
                    BroadcastNotificationId = message.BroadcastNotificationId
                };

                await _queueService.EnqueueAsync(targetQueueName, smsMessage);
                sentCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Failed to enqueue broadcast SMS for user {UserId} ({ModelType})",
                    contact.ModelId,
                    modelType);
            }
        }

        _logger.LogInformation(
            "Broadcast SMS chunk done. ModelType: {ModelType}, Skip: {Skip}, ChunkSize: {Chunk}, Queued: {SentCount}, Skipped: {SkippedCount}, BroadcastNotificationId: {BroadcastNotificationId}",
            targetRole,
            message.Skip,
            contacts.Count,
            sentCount,
            skippedCount,
            message.BroadcastNotificationId);

        if (contacts.Count == batchSize)
        {
            var next = new BroadcastNotificationQueueMessage
            {
                Channel = message.Channel,
                ModelType = message.ModelType,
                ModelIds = message.ModelIds,
                Subject = message.Subject,
                Body = message.Body,
                Data = message.Data,
                ClickAction = message.ClickAction,
                BroadcastNotificationId = message.BroadcastNotificationId,
                AttachmentLandingUrl = message.AttachmentLandingUrl,
                Skip = message.Skip + batchSize,
                BatchSize = batchSize
            };
            await _queueService.EnqueueAsync(queueName, next);
        }
    }
}
