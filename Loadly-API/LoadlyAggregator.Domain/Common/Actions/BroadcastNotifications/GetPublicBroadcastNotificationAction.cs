using LoadlyAggregator.Domain.Common.DTOs;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Actions.BroadcastNotifications;

public class GetPublicBroadcastNotificationAction
{
    private readonly IBroadcastNotificationRepository _broadcastNotificationRepository;
    private readonly IMediaService _mediaService;

    public GetPublicBroadcastNotificationAction(
        IBroadcastNotificationRepository broadcastNotificationRepository,
        IMediaService mediaService)
    {
        _broadcastNotificationRepository = broadcastNotificationRepository;
        _mediaService = mediaService;
    }

    public async Task<PublicBroadcastNotificationDto> ExecuteAsync(string publicToken)
    {
        BroadcastNotification broadcast = await _broadcastNotificationRepository.GetByPublicTokenAsync(publicToken)
            ?? throw new NotFoundException("notifications.broadcast_not_found");

        return new PublicBroadcastNotificationDto
        {
            Subject = broadcast.Subject,
            Body = broadcast.Body,
            SentAt = broadcast.CreatedAt,
            Attachments = broadcast.Attachments
                .Select(a => new PublicBroadcastNotificationAttachmentDto
                {
                    Id = a.Id,
                    Url = _mediaService.Read(a.Path)
                })
                .ToList()
        };
    }
}
