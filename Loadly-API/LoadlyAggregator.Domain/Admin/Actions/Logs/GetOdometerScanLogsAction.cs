using LoadlyAggregator.Domain.Admin.DTOs.Logs;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Logs;

public class GetOdometerScanLogsAction
{
    private readonly IOdometerScanLogRepository _odometerScanLogRepository;
    private readonly IMediaService _mediaService;

    public GetOdometerScanLogsAction(
        IOdometerScanLogRepository odometerScanLogRepository,
        IMediaService mediaService)
    {
        _odometerScanLogRepository = odometerScanLogRepository;
        _mediaService = mediaService;
    }

    public async Task<ListingDto<OdometerScanLogDto>> ExecuteAsync(GetOdometerScanLogsDto dto)
    {
        (var items, int totalCount) = await _odometerScanLogRepository.ListAsync(
            dto.TripId,
            dto.ModelId,
            dto.ModelType,
            dto.Status.HasValue ? (OdometerScanStatus)dto.Status.Value : null,
            dto.ScanContext.HasValue ? (OdometerScanContext)dto.ScanContext.Value : null,
            dto.IsScreen,
            dto.CorrelationId,
            dto.StartDate,
            dto.EndDate,
            dto.Page,
            dto.PageSize);

        return new ListingDto<OdometerScanLogDto>
        {
            Data = items
                .Select(item => OdometerScanLogDto.FromEntity(item, ResolveImageUrl(item.ImagePath)))
                .ToList(),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }

    private string? ResolveImageUrl(string? imagePath) =>
        string.IsNullOrWhiteSpace(imagePath) ? null : _mediaService.Read(imagePath);
}
