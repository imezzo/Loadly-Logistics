using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

public class DriverCompletedTripsByLifecycleActorDto
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int Driver { get; set; }
    public int Dispatcher { get; set; }
    public int Assisted { get; set; }
    public int Total { get; set; }
}

public class ListDriverCompletedTripsByLifecycleActorDto : BasePagedRequest
{
    public long? DriverId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
