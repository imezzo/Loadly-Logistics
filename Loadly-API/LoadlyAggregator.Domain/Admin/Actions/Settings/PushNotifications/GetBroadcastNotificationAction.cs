using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Settings.PushNotifications;

public class GetBroadcastNotificationAction
{
    private readonly IBroadcastNotificationRepository _broadcastNotificationRepository;
    private readonly IMediaService _mediaService;
    private readonly IBroadcastNotificationTargetResolver _targetResolver;

    public GetBroadcastNotificationAction(
        IBroadcastNotificationRepository broadcastNotificationRepository,
        IMediaService mediaService,
        IBroadcastNotificationTargetResolver targetResolver)
    {
        _broadcastNotificationRepository = broadcastNotificationRepository;
        _mediaService = mediaService;
        _targetResolver = targetResolver;
    }

    public async Task<BroadcastNotificationDto> ExecuteAsync(long id)
    {
        BroadcastNotification entity = await _broadcastNotificationRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("notifications.broadcast_not_found");

        BroadcastNotificationDto dto = BroadcastNotificationDto.FromEntity(entity);
        foreach (BroadcastNotificationAttachmentDto attachment in dto.Attachments)
        {
            attachment.Url = _mediaService.Read(attachment.Path);
        }

        List<long> ids = dto.Models.Select(m => m.Id).Distinct().ToList();
        if (ids.Count > 0)
        {
            IReadOnlyDictionary<long, string> names = await _targetResolver.GetUserNamesByIdsAsync(dto.ModelType, ids);
            foreach (BroadcastNotificationModelDto model in dto.Models)
            {
                model.Name = names.TryGetValue(model.Id, out string? name) ? name : string.Empty;
            }
        }

        return dto;
    }
}
