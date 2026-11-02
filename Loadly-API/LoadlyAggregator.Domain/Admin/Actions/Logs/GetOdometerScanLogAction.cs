using LoadlyAggregator.Domain.Admin.DTOs.Logs;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Logs;

public class GetOdometerScanLogAction : IAction<GetOdometerScanLogDto, OdometerScanLogDto>
{
    private readonly IOdometerScanLogRepository _odometerScanLogRepository;
    private readonly IMediaService _mediaService;

    public GetOdometerScanLogAction(
        IOdometerScanLogRepository odometerScanLogRepository,
        IMediaService mediaService)
    {
        _odometerScanLogRepository = odometerScanLogRepository;
        _mediaService = mediaService;
    }

    public async Task<OdometerScanLogDto> ExecuteAsync(GetOdometerScanLogDto dto)
    {
        var log = await _odometerScanLogRepository.GetByIdAsync(dto.Id)
            ?? throw new NotFoundException("odometer_scan_logs.not_found");

        return OdometerScanLogDto.FromEntity(
            log,
            string.IsNullOrWhiteSpace(log.ImagePath) ? null : _mediaService.Read(log.ImagePath),
            includeRawResponse: true);
    }
}
