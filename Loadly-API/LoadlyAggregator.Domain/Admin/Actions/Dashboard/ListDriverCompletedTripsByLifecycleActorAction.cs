using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Admin.Export;
using LoadlyAggregator.Domain.Common.Export;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Dashboard;

public class ListDriverCompletedTripsByLifecycleActorAction(
    ITripRepository tripRepository,
    ILanguageService languageService)
{
    public async Task<ListingDto<DriverCompletedTripsByLifecycleActorDto>> ExecuteAsync(ListDriverCompletedTripsByLifecycleActorDto dto)
    {
        var (rows, totalCount) = await tripRepository.GetDriverCompletedTripsByLifecycleActorAsync(
            dto.DriverId,
            dto.FromDate,
            dto.ToDate,
            dto.Page,
            dto.PageSize);

        return new ListingDto<DriverCompletedTripsByLifecycleActorDto>
        {
            Data = rows.Select(r => new DriverCompletedTripsByLifecycleActorDto
            {
                Id = r.Id,
                NameEn = r.NameEn,
                NameAr = r.NameAr,
                Phone = r.Phone,
                Driver = r.Driver,
                Dispatcher = r.Dispatcher,
                Assisted = r.Assisted,
                Total = r.Total
            }).ToList(),
            Additional = ExportListingMetadata.From(DriverLifecycleTripsExportColumns.All, languageService),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
