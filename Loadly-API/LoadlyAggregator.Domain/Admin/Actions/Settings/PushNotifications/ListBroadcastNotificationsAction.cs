using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Settings.PushNotifications;

public class ListBroadcastNotificationsAction
{
    private readonly IBroadcastNotificationRepository _broadcastNotificationRepository;
    private readonly IMediaService _mediaService;
    private readonly IBroadcastNotificationTargetResolver _targetResolver;

    public ListBroadcastNotificationsAction(
        IBroadcastNotificationRepository broadcastNotificationRepository,
        IMediaService mediaService,
        IBroadcastNotificationTargetResolver targetResolver)
    {
        _broadcastNotificationRepository = broadcastNotificationRepository;
        _mediaService = mediaService;
        _targetResolver = targetResolver;
    }

    public async Task<ListingDto<BroadcastNotificationDto>> ExecuteAsync(ListBroadcastNotificationsDto dto)
    {
        (List<BroadcastNotification> items, int totalCount) = await _broadcastNotificationRepository.ListAsync(
            dto.Search,
            dto.Page,
            dto.PageSize);

        List<BroadcastNotificationDto> data = items.ConvertAll(BroadcastNotificationDto.FromEntity);
        foreach (BroadcastNotificationDto item in data)
        {
            foreach (BroadcastNotificationAttachmentDto attachment in item.Attachments)
            {
                attachment.Url = _mediaService.Read(attachment.Path);
            }
        }

        await HydrateModelNamesAsync(data);

        return new ListingDto<BroadcastNotificationDto>
        {
            Data = data,
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }

    private async Task HydrateModelNamesAsync(List<BroadcastNotificationDto> data)
    {
        foreach (IGrouping<string, BroadcastNotificationDto> group in data.GroupBy(d => d.ModelType))
        {
            List<long> ids = group
                .SelectMany(d => d.Models.Select(m => m.Id))
                .Distinct()
                .ToList();
            if (ids.Count == 0)
            {
                continue;
            }

            IReadOnlyDictionary<long, string> names = await _targetResolver.GetUserNamesByIdsAsync(group.Key, ids);
            foreach (BroadcastNotificationDto notification in group)
            {
                foreach (BroadcastNotificationModelDto model in notification.Models)
                {
                    model.Name = names.TryGetValue(model.Id, out string? name) ? name : string.Empty;
                }
            }
        }
    }
}
