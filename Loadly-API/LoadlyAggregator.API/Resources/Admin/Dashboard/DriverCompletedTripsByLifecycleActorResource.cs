using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

namespace LoadlyAggregator.API.Resources.Admin.Dashboard;

public class DriverCompletedTripsByLifecycleActorResource
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int Driver { get; set; }
    public int Dispatcher { get; set; }
    public int Assisted { get; set; }
    public int Total { get; set; }

    public static DriverCompletedTripsByLifecycleActorResource FromDto(DriverCompletedTripsByLifecycleActorDto dto) =>
        new()
        {
            Id = dto.Id,
            NameEn = dto.NameEn,
            NameAr = dto.NameAr,
            Phone = dto.Phone,
            Driver = dto.Driver,
            Dispatcher = dto.Dispatcher,
            Assisted = dto.Assisted,
            Total = dto.Total
        };
}
