using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

public class ListVehicleAssignmentHistoriesDto : BasePagedRequest
{
    public long VehicleId { get; set; }
    public VehicleAssignmentHistorySource? Source { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}
