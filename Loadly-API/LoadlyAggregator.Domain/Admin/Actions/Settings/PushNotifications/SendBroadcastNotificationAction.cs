using System.Text.Json;
using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Shared.DTOs.Queue;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.Domain.Admin.Actions.Settings.PushNotifications;

public class SendBroadcastNotificationAction
{
    private readonly IQueueService _queueService;
    private readonly IBroadcastNotificationRepository _broadcastNotificationRepository;
    private readonly IBroadcastNotificationEnqueueOptions _broadcastEnqueueOptions;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediaService _mediaService;
    private readonly IBroadcastShareUrlBuilder _shareUrlBuilder;
    private readonly ILogger<SendBroadcastNotificationAction> _logger;

    private const string BroadcastQueueName = "admin";
    private const string AttachmentFolderPrefix = "broadcast-notifications";

    public SendBroadcastNotificationAction(
        IQueueService queueService,
        IBroadcastNotificationRepository broadcastNotificationRepository,
        IBroadcastNotificationEnqueueOptions broadcastEnqueueOptions,
        ICurrentUserService currentUserService,
        IMediaService mediaService,
        IBroadcastShareUrlBuilder shareUrlBuilder,
        ILogger<SendBroadcastNotificationAction> logger)
    {
        _queueService = queueService;
        _broadcastNotificationRepository = broadcastNotificationRepository;
        _broadcastEnqueueOptions = broadcastEnqueueOptions;
        _currentUserService = currentUserService;
        _mediaService = mediaService;
        _shareUrlBuilder = shareUrlBuilder;
        _logger = logger;
    }

    public async Task<BroadcastNotificationResultDto> ExecuteAsync(SendBroadcastNotificationDto dto)
    {
        string modelType = dto.ModelType.ToLowerInvariant();

        BroadcastNotification broadcastRecord = new()
        {
            AddedBy = _currentUserService.GetCurrentUserId() ?? 0,
            Channel = dto.Channel,
            Subject = dto.Subject,
            Body = dto.Body,
            ModelType = modelType,
            ModelIds = dto.ModelIds is { Count: > 0 }
                ? JsonSerializer.Serialize(dto.ModelIds)
                : null,
            PublicToken = CodeGenerator.GenerateUrlSafeToken()
        };

        await _broadcastNotificationRepository.CreateAsync(broadcastRecord);
        await _broadcastNotificationRepository.SaveChangesAsync();

        await PersistAttachmentsAsync(broadcastRecord, dto.Attachments);

        string? landingUrl = broadcastRecord.Attachments.Count > 0
            ? _shareUrlBuilder.Build(broadcastRecord.PublicToken)
            : null;

        var message = new BroadcastNotificationQueueMessage
        {
            Channel = dto.Channel.ToNotificationChannel(),
            ModelType = modelType,
            ModelIds = dto.ModelIds,
            Subject = dto.Subject,
            Body = dto.Body,
            ClickAction = dto.ClickAction,
            BroadcastNotificationId = broadcastRecord.Id,
            AttachmentLandingUrl = landingUrl,
            Skip = 0,
            BatchSize = _broadcastEnqueueOptions.BatchSize
        };

        await _queueService.EnqueueAsync(BroadcastQueueName, message);

        _logger.LogInformation(
            "Broadcast notification queued: Channel={Channel}, ModelType={ModelType}, BroadcastNotificationId={BroadcastNotificationId}",
            dto.Channel, modelType, broadcastRecord.Id);

        return new BroadcastNotificationResultDto
        {
            Queued = true,
            Message = "notifications.broadcast_queued"
        };
    }

    private async Task PersistAttachmentsAsync(BroadcastNotification broadcastRecord, List<FileUpload>? attachments)
    {
        if (attachments is not { Count: > 0 })
        {
            return;
        }

        List<string> paths = await _mediaService.UploadImagesAsync(
            attachments,
            $"{AttachmentFolderPrefix}/{broadcastRecord.Id}");

        foreach (string path in paths)
        {
            broadcastRecord.Attachments.Add(new BroadcastNotificationAttachment { Path = path });
        }

        await _broadcastNotificationRepository.SaveChangesAsync();
    }
}
